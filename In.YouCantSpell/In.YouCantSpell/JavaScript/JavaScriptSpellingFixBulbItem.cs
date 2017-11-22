using System;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Services;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.TextControl;
using YouCantSpell.Utility;
using JetBrains.ReSharper.Psi.JavaScript.Parsing;

namespace YouCantSpell.ReSharper.JavaScript
{
	public class JavaScriptSpellingFixBulbItem : SpellingFixBulbItemBase<JavaScriptSpellingErrorHighlighting>
	{
		public JavaScriptSpellingFixBulbItem(JavaScriptSpellingErrorHighlighting highlighting, string suggestion)
			: base(highlighting, suggestion) { }

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			if(!Highlighting.IsValid())
				return null;

			var node = Highlighting.Node;
			var badWordTextRange = Highlighting.Range.TextRange;

			var newText = StringUtil.ReplaceSection(
				node.GetText(),
				Suggestion,
				badWordTextRange.StartOffset - node.GetDocumentRange().TextRange.StartOffset,
				badWordTextRange.Length
			);

			if(node is IIdentifier) {
				return textControl => {
					var declaredIdentifier = TextControlToPsi.GetDeclaredElements(solution, textControl)
						.FirstOrDefault();
					solution.GetComponent<RenameRefactoringExecutor>().Execute(declaredIdentifier, textControl, newText);
				};
			}
			if(node is IComment) {
				var newElement = CreateNode(node, newText);
				ModificationUtil.ReplaceChild(node, newElement);
				return null;
			}
			if (JavaScriptUtil.IsStringLiteral(node)) {
				var newElement = CreateNode(node, newText);
				ModificationUtil.ReplaceChild(node, newElement);
				return null;
			}
			return null;
		}

		private static ITreeNode CreateNode(ITreeNode basedOn, string text) {
			var languageService = basedOn.Language.LanguageService();
			if(null == languageService)
				return null;
			var parser = languageService.CreateParser(
				languageService.GetPrimaryLexerFactory().CreateLexer(new StringBuffer(text)),
				basedOn.GetPsiModule(),
				basedOn.GetSourceFile()
			) as IJavaScriptParser;
			if (null != parser) {
				var newNodes = parser.ParseFile();
				if(null != newNodes)
					return newNodes.LastChild;
			}
			return null;
		}

	}
}
