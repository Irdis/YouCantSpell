using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;
using YouCantSpell.CStyle;

namespace YouCantSpell.ReSharper
{
	public abstract class TagMarkupSpellCheckDaemonStageProcessBase<TCodeFile>
		: IDaemonStageProcess
		where TCodeFile : class, IFile
	{

		private const int MinWordSize = 2;

		protected TagMarkupSpellCheckDaemonStageProcessBase(IDaemonProcess daemonProcess, TCodeFile codeFile)
		{
			if(null == daemonProcess) throw new ArgumentNullException("daemonProcess");
			if(null == codeFile) throw new ArgumentNullException("codeFile");
			DaemonProcess = daemonProcess;
			CodeFile = codeFile;

			SpellCheckResources = Shell.Instance.GetComponent<ShellSpellCheckResources>();
			PsiSourceFile = daemonProcess.SourceFile;
			Document = daemonProcess.Document;
		}

		public IDaemonProcess DaemonProcess { get; private set; }

		public TCodeFile CodeFile { get; private set; }

		public ShellSpellCheckResources SpellCheckResources { get; private set; }

		public IPsiSourceFile PsiSourceFile { get; private set; }

		public IDocument Document { get; private set; }

		protected DocumentRange? FindTrueDocumentRange(TreeTextRange range) {
			var ranges = CodeFile.GetIntersectingRanges(range)
				.Where(x => x.Document == PsiSourceFile.Document)
				.ToList();
			return ranges.Count == 0 ? (DocumentRange?)null : ranges[0];
		}

		protected bool WordIsIgnored(string word) {
			return String.IsNullOrEmpty(word)
				|| word.Length < MinWordSize
				|| SpellCheckResources.IsIgnoredInsensitive(word)
				|| CStyleFreeTextParser.LooksLikeCodeWord(word);
		}

		protected IEnumerable<HighlightingInfo> FindWordHighlightings(ITreeNode node) {
			var validRange = FindTrueDocumentRange(node.GetTreeTextRange());
			if(!validRange.HasValue) return Enumerable.Empty<HighlightingInfo>();

			return FindWordHighlightings(node, node.GetText(), validRange.Value);
		}

		protected IEnumerable<HighlightingInfo> FindWordHighlightings(ITreeNode node, string text, DocumentRange textRange) {
			var highlights = new List<HighlightingInfo>(0);
			if(WordIsIgnored(text))
				return highlights;

			var parser = new CStyleFreeTextParser();
			var wordParts = parser.ParseSentenceWordsForSpellCheck(new TextSubString(text));
			foreach(var wordPart in wordParts) {
				var word = wordPart.SubText;

				// Make sure that the word is not to be ignored for any reason.
				if(WordIsIgnored(word)) continue;

				// Finally we check the spelling of the word.
				if(SpellCheckResources.SpellChecker.Check(word)) continue;

				// If we got this far we need to offer spelling suggestions.
				var wordPartDocumentOffset = textRange.TextRange.StartOffset + wordPart.Offset;
				var wordRange = new DocumentRange(Document, new TextRange(wordPartDocumentOffset, wordPartDocumentOffset + word.Length));
				var highlight = CreateHighlighting(node, wordRange, word, null);
				highlights.Add(new HighlightingInfo(wordRange, highlight));
			}
			return highlights;
		}

		public abstract void Execute(Action<DaemonStageResult> committer);

		protected abstract SpellingErrorHighlightingBase CreateHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions);

	}
}
