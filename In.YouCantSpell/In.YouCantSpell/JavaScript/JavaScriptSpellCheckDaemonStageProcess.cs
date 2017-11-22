using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.JavaScript.Impl.Tree;
using JetBrains.ReSharper.Psi.JavaScript.LanguageImpl;
using JetBrains.ReSharper.Psi.JavaScript.Parsing;
using JetBrains.ReSharper.Psi.JavaScript.Tree;
using JetBrains.ReSharper.Psi.Tree;


namespace YouCantSpell.ReSharper.JavaScript
{
	public class JavaScriptSpellCheckDaemonStageProcess : CStyleSpellCheckDaemonStageProcessBase<IJavaScriptFile>
	{

		/// <summary>
		/// Determines if an node node is acceptable for spell checking.
		/// </summary>
		/// <param name="node">The node to test.</param>
		/// <returns>True when the node may be spell checked.</returns>
		private static bool IsValidSpellCheckIdentifierNode(ITreeNode node) {
			if(null == node || null == node.Parent || null == node.Parent.Parent)
				return false;
			return IsValidSpellCheckIdentifierNodeSingleCheck(node)
				&& IsValidSpellCheckIdentifierNodeSingleCheck(node.Parent)
				&& IsValidSpellCheckIdentifierNodeSingleCheck(node.Parent.Parent);
		}

		private static bool IsValidSpellCheckIdentifierNodeSingleCheck(ITreeNode node) {
			if(null == node)
				return false;
			if(node is IReferenceExpression)
				return false;
			return true;
		}

		public JavaScriptSpellCheckDaemonStageProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, IJavaScriptFile javaScriptFile)
			: base(process,settingsStore,javaScriptFile,JavaScriptLanguage.Instance)
		{ }

		protected override SpellingErrorHighlightingBase CreateErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions) {
			return new JavaScriptSpellingErrorHighlighting(node,errorRange,wordInError,getSuggestions);
		}

		protected override bool IsIgnored(string text) {
			return base.IsIgnored(text) || JavaScriptUtil.IsKeyword(text);
		} 

		protected override IEnumerable<IType> GetRelatedTypes(IDeclaration declaration) {
			if(declaration is ITypeDeclaration)
				return GetSuperTypes(declaration as ITypeDeclaration).Cast<IType>();
			return Enumerable.Empty<IType>();
		}

		public override void Execute(Action<DaemonStageResult> committer) {
			var localIdentifierNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
			var highlightings = new List<HighlightingInfo>();

			CodeFile.ProcessChildren<IIdentifier>(
				node => {
					localIdentifierNames.Add(node.Name);
					localIdentifierNames.Add(RemovePrefixAndSuffix(node));
					if(IsValidSpellCheckIdentifierNode(node)) {
						highlightings.AddRange(FindHighlightings(node));
					}
				}
			);

#if (RSHARP6 || RESHARP7)
			var declarationsCache = CodeFile.GetPsiServices().CacheManager.GetDeclarationsCache(CodeFile.GetPsiModule(), true, true);
#else
            var declarationsCache = CodeFile.GetPsiServices().Symbols;
#endif

			CodeFile.ProcessChildren<ITreeNode>(
				node => {
#if RSHARP6
					if (node is JavaScriptGenericToken && (node as JavaScriptGenericToken).NodeType == JavaScriptTokenType.STRING_LITERAL) {
						highlightings.AddRange(FindStringHighlightings(node, localIdentifierNames, declarationsCache));
					}
#else
					if (node is IJavaScriptLiteralExpression && (node as IJavaScriptLiteralExpression).ConstantValueType == ConstantValueTypes.String) {
						highlightings.AddRange(FindStringHighlightings(node, localIdentifierNames, declarationsCache));
					}
#endif
					else if(node is IComment) {
						highlightings.AddRange(FindHighlightings(node as IComment, localIdentifierNames, declarationsCache));
					}
				}
			);

			committer(new DaemonStageResult(highlightings));
		}

		public override bool IsValidIdentifierForSpellCheck(ITreeNode node)
		{
			return IsValidSpellCheckIdentifierNode(node);
		}
	}
}
