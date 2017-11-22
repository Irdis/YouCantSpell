using System;
using System.Linq;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Html.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using YouCantSpell.Utility;

namespace YouCantSpell.ReSharper.Html
{
	public class HtmlSpellingFixBulbItem : SpellingFixBulbItemBase<HtmlSpellingErrorHighlighting>
	{

		public HtmlSpellingFixBulbItem(HtmlSpellingErrorHighlighting highlighting, string suggestion)
			: base(highlighting, suggestion) { }

		protected override Action<JetBrains.TextControl.ITextControl> ExecutePsiTransaction(JetBrains.ProjectModel.ISolution solution, JetBrains.Application.Progress.IProgressIndicator progress) {
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

			if(node is TreeElement)
			{
				var elementFactory = HtmlElementFactory.GetInstance(node.Language);
				var newElements = elementFactory.CompileText(newText, node).ToList();
				if(newElements.Count > 0)
				{
					var recentNode = ModificationUtil.ReplaceChild(node, newElements[0]);
					for(int i = 1; i < newElements.Count; i++) {
						recentNode = ModificationUtil.AddChildAfter(recentNode.Parent, recentNode, newElements[i]);
					}
				}
				return null;
			}
			return null;
		}

	}
}
