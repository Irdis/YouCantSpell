using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Application;
using JetBrains.DocumentModel;
#if (RSHARP8)
using JetBrains.Metadata.Reader.API;
#endif
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Naming;
using JetBrains.ReSharper.Psi.Naming.Interfaces;
using JetBrains.ReSharper.Psi.Naming.Settings;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Application.Settings;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Naming.Impl;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using YouCantSpell.CStyle;
using YouCantSpell.ReSharper.CSharp;
using YouCantSpell.Utility;

namespace YouCantSpell.ReSharper
{
	public abstract class CStyleSpellCheckDaemonStageProcessBase<TCodeFile>
		: IDaemonStageProcess
		where TCodeFile : class, IFile
	{

		private const int MinWordSize = 2;

		/// <summary>
		/// Force the casing of the first character in a string to the desired case.
		/// </summary>
		/// <param name="text">The text to adjust.</param>
		/// <param name="textCase">The case to adjust the first letter to.</param>
		/// <returns>The adjusted string.</returns>
		protected static string ForceFirstCharCase(string text, TextCaseClassification textCase) {
			if(String.IsNullOrEmpty(text))
				return text;

			char firstLetter;
			switch(textCase) {
			case TextCaseClassification.Upper: { firstLetter = Char.ToUpper(text[0]); break; }
			case TextCaseClassification.Lower: { firstLetter = Char.ToLower(text[0]); break; }
			default: return text;
			}

			if(firstLetter == text[0])
				return text; // it was already correct, so just return it

			return String.Concat(firstLetter, text.Length > 1 ? text.Substring(1) : String.Empty);

		}

		/// <summary>
		/// Determines the case of the first letter of the second word within an identifier of the given name style.
		/// </summary>
		/// <param name="kind">The name style.</param>
		/// <returns>The case of the first letter of the second word within an identifier.</returns>
		protected static TextCaseClassification SecondWordFirstLetterClassification(NamingStyleKinds kind) {
			switch(kind) {
			case NamingStyleKinds.AaBb:
			case NamingStyleKinds.AA_BB:
			case NamingStyleKinds.aaBb:
				return TextCaseClassification.Upper;
			case NamingStyleKinds.Aa_bb:
			case NamingStyleKinds.aa_bb:
				return TextCaseClassification.Lower;
			default:
				return TextCaseClassification.Unknown;
			}
		}

		private struct ParseKey : IEquatable<ParseKey>
		{
			public readonly NamingRule NamingRule;
			public readonly string Text;

			public ParseKey(string text, NamingRule namingRule) {
				Text = text;
				NamingRule = namingRule;
			}

			public override bool Equals(object obj) {
				return obj is ParseKey && Equals((ParseKey)obj);
			}

			public bool Equals(ParseKey other) {
				return String.Equals(Text, other.Text) && NamingRule.Equals(other.NamingRule);
			}

			public override int GetHashCode() {
				return Text.GetHashCode() ^ -NamingRule.GetHashCode();
			}
		}

		private readonly Dictionary<ParseKey, Name> _nameParseCache = new Dictionary<ParseKey, Name>();
		private readonly ReaderWriterLockSlim _nameParseCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		protected CStyleSpellCheckDaemonStageProcessBase(
			IDaemonProcess process,
			IContextBoundSettingsStore settingsStore,
			TCodeFile codeFile,
			PsiLanguageType languageType
		) {
			if(null == process) throw new ArgumentNullException("process");
			if(null == settingsStore) throw new ArgumentNullException("settingsStore");
			if(null == codeFile) throw new ArgumentNullException("codeFile");
			if(null == languageType) throw new ArgumentNullException("languageType");

			DaemonProcess = process;
			SettingsStore = settingsStore;
			CodeFile = codeFile;
			LanguageType = languageType;
			PsiSourceFile = process.SourceFile;
			SpellCheckResources = Shell.Instance.GetComponent<ShellSpellCheckResources>();
			Document = process.Document;
			var psiServices = process.SourceFile.PsiModule.GetPsiServices();
			NamingManager = psiServices.Naming;
			NamingPolicyProvider = psiServices.Naming.Policy.GetPolicyProvider(languageType, process.SourceFile, settingsStore);
		}

		public IDaemonProcess DaemonProcess { get; private set; }

		public IContextBoundSettingsStore SettingsStore { get; private set; }

		public TCodeFile CodeFile { get; private set; }

		public IPsiSourceFile PsiSourceFile { get; private set; }

		public IDocument Document { get; private set; }

		public ShellSpellCheckResources SpellCheckResources { get; private set; }

		public NamingManager NamingManager { get; private set; }

		public INamingPolicyProvider NamingPolicyProvider { get; private set; }

		public PsiLanguageType LanguageType { get; private set; }

		public abstract void Execute(Action<DaemonStageResult> committer);

		private Name ParseName(string name, NamingRule namingRule) {
			Name result;
			var key = new ParseKey(name, namingRule);
			_nameParseCacheLock.EnterUpgradeableReadLock();
			try {
				if (!_nameParseCache.TryGetValue(key, out result)) {
					_nameParseCacheLock.EnterWriteLock();
					try {
						result = NamingManager.Parsing.Parse(name, namingRule, NamingPolicyProvider);
						_nameParseCache.Add(key, result);
					}
					finally {
						_nameParseCacheLock.ExitWriteLock();
					}
				}
			}
			finally {
				_nameParseCacheLock.ExitUpgradeableReadLock();
			}
			return result;
		}

		protected Name ParseName(string name) {
			return ParseName(name, NamingRule.Default);
		}

		protected Name ParseName(IIdentifier identifier) {
			var declared = ReSharperUtil.GetDeclaredElement(identifier);
			return ParseName(identifier.Name,null == declared ? NamingRule.Default : NamingPolicyProvider.GetPolicy(declared).NamingRule);
		}

		protected IEnumerable<IDeclaredType> GetSuperTypes(ITypeDeclaration typeElement) {
			if (null == typeElement)
				return Enumerable.Empty<IDeclaredType>();
			return typeElement.SuperTypes;
		}

		protected DocumentRange? FindTrueDocumentRange(TreeTextRange range) {
			var ranges = CodeFile.GetIntersectingRanges(range)
				.Where(x => x.Document == PsiSourceFile.Document)
				.ToList();
			return ranges.Count == 0 ? (DocumentRange?)null : ranges[0];
		}

		protected IEnumerable<TextSubString> ParseNameParts(string nameText) {
			var parsedName = ParseName(nameText);
			if(parsedName.InnerElements == null || parsedName.InnerElements.Count == 0)
				yield break;

			int identifierNamePartIndex = null != parsedName.NamePrefix
				? parsedName.NamePrefix.Text.Length
				: 0;

			foreach(var identifierNamePart in parsedName.InnerElements) {
				if(!identifierNamePart.IsSeparator)
					yield return new TextSubString(nameText, identifierNamePartIndex, identifierNamePart.Text.Length);

				identifierNamePartIndex += identifierNamePart.Text.Length;
			}
		}

		protected virtual bool IsIgnored(string text)
		{
			return String.IsNullOrEmpty(text)
				|| text.Length <= MinWordSize
				|| SpellCheckResources.IsIgnoredInsensitive(text);
		}

		protected string RemovePrefixAndSuffix(IIdentifier identifier) {
			var name = ParseName(identifier);
			var nameParts = name.InnerElements.ToList();
			int startIndex = (nameParts.Count > 0 && nameParts[0].IsSeparator)
				? 1
				: 0;
			int endIndex = (nameParts.Count > 0 && nameParts[nameParts.Count - 1].IsSeparator)
				? nameParts.Count - 1
				: nameParts.Count;

			if (startIndex >= endIndex) return String.Empty;

			if (startIndex == 0 && endIndex == nameParts.Count)
				return identifier.Name;

			var builder = new StringBuilder();
			for(int i = startIndex; i < endIndex; i++)
				builder.Append(nameParts[i].Text);
			return builder.ToString();
		}

		protected virtual IEnumerable<HighlightingInfo> FindFreeTextHighlightings(
          ITreeNode node, 
          string text, 
          DocumentRange textRange, 
          HashSet<string> localIdentifierNames, 
#if (RSHARP6 || RSHARP7)
          IDeclarationsCache declarationsCache
#else
          ISymbolCache declarationsCache
#endif
          ) {
			var highlights = new List<HighlightingInfo>(0);
			if(IsIgnored(text))
				return highlights;

			var parser = new CStyleFreeTextParser();
			var xmlTextParts = parser.ParseXmlTextParts(new TextSubString(text));
			var wordParts = xmlTextParts.SelectMany(parser.ParseSentenceWordsForSpellCheck);
			foreach(var wordPart in wordParts) {
				var word = wordPart.SubText;

				// Make sure that the word is not to be ignored for any reason.
				if(IsIgnored(word))
					continue;
				if(CStyleFreeTextParser.LooksLikeCodeWord(word) || localIdentifierNames.Contains(word))
					continue;
#if (RSHARP6 || RSHARP7)
				if(declarationsCache.GetElementsByShortName(word).NotNullAndHasAny())
					continue;
#else
                if (declarationsCache.GetSymbolScope(LibrarySymbolScope.FULL, false).GetElementsByShortName(word).NotNullAndHasAny())
					continue;
#endif

				// Finally we check the spelling of the word.
				if(SpellCheckResources.SpellChecker.Check(word))
					continue;

				// If we got this far we need to offer spelling suggestions.
				var wordPartDocumentOffset = textRange.TextRange.StartOffset + wordPart.Offset;
				var wordRange = new DocumentRange(
					Document,
					new TextRange(wordPartDocumentOffset, wordPartDocumentOffset + word.Length)
				);
				highlights.Add(new HighlightingInfo(wordRange, CreateErrorHighlighting(node,wordRange,word,null)));
			}
			return highlights;
		}

		protected IEnumerable<HighlightingInfo> FindHighlightings(
          IComment comment, 
          HashSet<string> localIdentifierNames,
#if (RSHARP6 || RSHARP7)
          IDeclarationsCache declarationsCache
#else
          ISymbolCache declarationsCache
#endif
          )
        {
			var fullText = comment.CommentText;
			if(String.IsNullOrEmpty(fullText) || ReSharperUtil.ReSharperLineRegex.IsMatch(fullText))
				return Enumerable.Empty<HighlightingInfo>(); // ignore resharper disable/restore lines

			var validRange = FindTrueDocumentRange(comment.GetCommentRange());
			if(!validRange.HasValue)
				return Enumerable.Empty<HighlightingInfo>();
			var documentRange = validRange.Value;

			return FindFreeTextHighlightings(comment, fullText, documentRange, localIdentifierNames, declarationsCache);
		}

		protected IEnumerable<HighlightingInfo> FindStringHighlightings(
			ITreeNode node,
			HashSet<string> localIdentifierNames,
#if (RSHARP6 || RSHARP7)
            IDeclarationsCache declarationsCache
#else
            ISymbolCache declarationsCache
#endif
		) {
			var validRange = FindTrueDocumentRange(node.GetTreeTextRange());
			if(!validRange.HasValue)
				return Enumerable.Empty<HighlightingInfo>();
			var textRange = validRange.Value;

			var coreMatch = CSharpUtil.StringLiteralContentParser.Match(node.GetText());
			if(!coreMatch.Success)
				return Enumerable.Empty<HighlightingInfo>();

			var coreMatchGroup = coreMatch.Groups[1];
			if(coreMatchGroup.Index > 0)
				textRange = textRange.SetStartTo(textRange.TextRange.StartOffset + coreMatchGroup.Index);

			return FindFreeTextHighlightings(node, coreMatchGroup.Value, textRange, localIdentifierNames, declarationsCache);
		}

		private IEnumerable<string> ProposeSuggestions(string[] suggestionWords, NamingRule namingRule, TextCaseClassification textCaseClassification) {
			// Use resharper to reformat the word parts
			var normalSuggestion = NamingManager.Parsing.RenderNameSafe(
				NameRoot.FromWords(Emphasis.Unknown, false, suggestionWords),
				namingRule,
				LanguageType,
				NamingPolicyProvider
			);

			if (textCaseClassification == TextCaseClassification.Upper && namingRule.NamingStyleKind != NamingStyleKinds.AA_BB) {
				normalSuggestion = normalSuggestion.ToUpper();
				yield return NamingManager.Parsing.RenderNameSafe(
					NameRoot.FromWords(Emphasis.Unknown, false, suggestionWords),
					ReSharperUtil.OgreCaps,
					LanguageType,
					NamingPolicyProvider
				);
			}

			yield return normalSuggestion;
		}

		private string MassageProposedSuggestion(string newSuggestion, bool literalIsOk, int position, NamingRule namingRule) {
			// after rendering the suggestion as an identifier name we need to parse it again to remove some stuff
			// just in case anything funny like a prefix or @ was added we need to get rid of that (unless @ is OK)
			var reParsedNamed = NamingManager.Parsing.Parse(newSuggestion, NamingRule.Default, NamingPolicyProvider);
			if (reParsedNamed.NamePrefix.Text.Length > 0 && (!literalIsOk || reParsedNamed.NamePrefix.Text != "@")) {
				// we need to pull the prefix off except for one special situation, with literals, if that is a prefix
				newSuggestion = newSuggestion.Substring(reParsedNamed.NamePrefix.Text.Length);
			}
			if (!literalIsOk && newSuggestion.Length > 1 && newSuggestion.StartsWith("@"))
				newSuggestion = newSuggestion.Substring(1); // if the @ prefix is not OK we need to get rid of that

			// the first word within an identifier often has special casing that differs from the other words: firstSecondThird
			if (!String.IsNullOrEmpty(newSuggestion) && 0 != position) {
				// Here we may need to correct the first letter if this is not the first word as the naming provider assumes the first word.
				// The R# methods assume the words I am giving it are the entire identifier name so the first letter may not be correct for
				// a suggestion derived from the 2nd word part within an identifier, we will need to adjust that.
				newSuggestion = ForceFirstCharCase(
					newSuggestion,
					SecondWordFirstLetterClassification(namingRule.NamingStyleKind)
				);
			}
			return newSuggestion;
		}

		public IEnumerable<string> FormatSuggestions(IEnumerable<string> suggestions, int position, int wordCount, NamingRule namingRule, TextCaseClassification textCaseClassification) {
			var literalIsOk = wordCount == 1; // literals (@object) are only OK if there is one word in the identifier

			foreach(var suggestion in suggestions) {
				
				// Get the word parts of the suggestion.
				var matches = Utility.StringUtil.LetterParserRegex.Matches(suggestion).Cast<Match>().Select(x => x.Value).ToArray();
				if (matches.Length == 0) {
					yield return suggestion;
					continue;
				}

				var newProposedSuggestions = ProposeSuggestions(matches, namingRule, textCaseClassification);

				if (textCaseClassification != TextCaseClassification.Unknown)
					newProposedSuggestions = newProposedSuggestions.Select(x => MassageProposedSuggestion(x, literalIsOk, position, namingRule));

				foreach (var newSuggestion in newProposedSuggestions) {
					yield return newSuggestion;
				}
			}
		}

        protected virtual IEnumerable<string> GeneratePresentableNameParts(IType type)
        {
            var fullPresentableName = type.GetPresentableName(LanguageType);
            yield return fullPresentableName;
            foreach (var match in Utility.StringUtil.WordParserRegex.Matches(fullPresentableName).Cast<Match>())
            {
                yield return match.Value;
            }

        }

		protected IEnumerable<HighlightingInfo> FindHighlightings(IIdentifier identifier) {
			var results = new List<HighlightingInfo>(0);

			var declaration = ReSharperUtil.GetDeclaration(identifier);
			if(null == declaration)
				return results;

			var declared = declaration.DeclaredElement;
			if(null == declared)
				return results;

			var validRange = FindTrueDocumentRange(identifier.GetTreeTextRange());
			if(!validRange.HasValue)
				return results;

			var validPartRange = validRange.Value;

			var localIgnoredWords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			var relatedTypeNames = GetRelatedTypes(declaration)
                .SelectMany(GeneratePresentableNameParts)
				.ToList();

			localIgnoredWords.AddRange(relatedTypeNames);
			localIgnoredWords.AddRange(relatedTypeNames.SelectMany(ParseNameParts).Select(x => x.SubText));

			if(IsIgnored(identifier.Name) || localIgnoredWords.Contains(identifier.Name))
				return results;

			var parsedNameParts = ParseNameParts(identifier.Name).ToList();
			var namingRule = NamingPolicyProvider.GetPolicy(declared).NamingRule;
			var textClassification = Utility.StringUtil.ClassifyCharCase(identifier.Name);

			for(int i = 0; i < parsedNameParts.Count; i++) {
				var namePart = parsedNameParts[i];
				var wordPart = Utility.StringUtil.LetterParserRegex
					.Matches(namePart.SubText)
					.Cast<Match>()
					.SingleOrDefault(x => x.Success);

				if(null == wordPart || IsIgnored(wordPart.Value) || localIgnoredWords.Contains(wordPart.Value))
					continue;

				if(SpellCheckResources.SpellChecker.Check(wordPart.Value))
					continue;

				var identifierLeftTrim = namePart.Offset + wordPart.Index;
				var localValidPartRange = (identifierLeftTrim > 0)
					? validPartRange.TrimLeft(identifierLeftTrim)
					: validPartRange;

				localValidPartRange = localValidPartRange.SetEndTo(localValidPartRange.TextRange.StartOffset + wordPart.Value.Length);

				int wordPosition = i;
				results.Add(new HighlightingInfo(
					localValidPartRange,
					CreateErrorHighlighting(
						identifier,
						localValidPartRange,
						wordPart.Value,
						word => FormatSuggestions(
								Shell.Instance.GetComponent<ShellSpellCheckResources>().SpellChecker.GetRecommendations(word),
								wordPosition,
								parsedNameParts.Count,
								namingRule,
								textClassification
							)
							//.Select(x => FormatSuggestion(x, wordPosition, namingRule, parsedNameParts.Count, textClassification))
							.Where(x => !String.IsNullOrEmpty(x) && word != x)
							.Distinct()
							.Take(ReSharperUtil.MaxSuggestions)
							.ToArray()
					)
				));
			}
			return results;

		}

		protected abstract SpellingErrorHighlightingBase CreateErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions);

		public abstract bool IsValidIdentifierForSpellCheck(ITreeNode node);

		protected abstract IEnumerable<IType> GetRelatedTypes(IDeclaration declaration);


	}
}
