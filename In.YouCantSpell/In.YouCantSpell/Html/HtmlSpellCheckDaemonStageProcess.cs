using System;
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Html.Impl.Tree;
using JetBrains.ReSharper.Psi.Html.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace YouCantSpell.ReSharper.Html
{
	public class HtmlSpellCheckDaemonStageProcess : TagMarkupSpellCheckDaemonStageProcessBase<IHtmlFile>
	{

		public HtmlSpellCheckDaemonStageProcess(IDaemonProcess daemonProcess, IHtmlFile htmlFile)
			:base(daemonProcess, htmlFile) { }

		private IEnumerable<HighlightingInfo> FindHighlightings(HtmlTag node) {
			var results = new List<HighlightingInfo>(0);
			foreach(var attribute in node.Attributes) {
				var valueElement = attribute.ValueElement;
				if(null == valueElement) continue;

				var validRange = FindTrueDocumentRange(valueElement.GetUnquotedTreeTextRange());
				if (!validRange.HasValue) continue;

				results.AddRange(FindWordHighlightings(valueElement,valueElement.UnquotedValue,validRange.Value));
			}
			return results;
		}

		protected override SpellingErrorHighlightingBase CreateHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions) {
			return new HtmlSpellingErrorHighlighting(node, errorRange, wordInError, null);
		}

		public override void Execute(Action<DaemonStageResult> committer) {
			var textNodeTypes = CodeFile.TokenTypes.TEXT;
			var highlightings = new List<HighlightingInfo>(0);

			CodeFile.ProcessChildren<HtmlTag>(node =>
				highlightings.AddRange(FindHighlightings(node))
			);

			CodeFile.ProcessChildren<TreeElement>(node => {
				if(textNodeTypes.Equals(node.NodeType))
					highlightings.AddRange(FindWordHighlightings(node));
			});

			committer(new DaemonStageResult(highlightings));
		}

		
	}
}
