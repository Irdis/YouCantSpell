using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.Impl.Tree;
using JetBrains.Text;
using JetBrains.TextControl;
using YouCantSpell.Utility;

namespace YouCantSpell.ReSharper.Xml
{
	public class XmlSpellingFixBulbItem : SpellingFixBulbItemBase<XmlSpellingErrorHighlighting>
	{

		public XmlSpellingFixBulbItem(XmlSpellingErrorHighlighting highlighting, string suggestion)
			: base(highlighting, suggestion) { }

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress) {
			if (!Highlighting.IsValid())
				return null;

			var node = Highlighting.Node;
			var badWordTextRange = Highlighting.Range.TextRange;

			var newText = StringUtil.ReplaceSection(
				node.GetText(),
				Suggestion,
				badWordTextRange.StartOffset - node.GetDocumentRange().TextRange.StartOffset,
				badWordTextRange.Length
			);

			if(node is XmlValueToken) {
				var newElm = new XmlValueToken(
					(node as XmlValueToken).GetTokenType(),
					new StringBuffer(newText), 
					new TreeOffset(0), 
					new TreeOffset(newText.Length)
				);
				ModificationUtil.ReplaceChild(node, newElm);
				return null;
			}
			if (node is XmlFloatingTextToken) {
				var newElm = new XmlFloatingTextToken(
					(node as XmlFloatingTextToken).GetTokenType(),
					newText
				);
				ModificationUtil.ReplaceChild(node, newElm);
				return null;
			}
			if (node is XmlToken) {
				var newElm = new XmlToken(
					(node as XmlToken).GetTokenType(),
					new StringBuffer(newText),
					new TreeOffset(0),
					new TreeOffset(newText.Length)
				);
				ModificationUtil.ReplaceChild(node, newElm);
				return null;
			}
			return null;
		}

	}
}
