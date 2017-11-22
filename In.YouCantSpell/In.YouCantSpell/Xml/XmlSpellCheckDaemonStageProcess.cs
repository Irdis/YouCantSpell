using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml.Impl.Tree;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace YouCantSpell.ReSharper.Xml
{
    public class XmlSpellCheckDaemonStageProcess : TagMarkupSpellCheckDaemonStageProcessBase<IXmlFile>
	{

		public XmlSpellCheckDaemonStageProcess(IDaemonProcess daemonProcess, IXmlFile xmlFile)
			: base(daemonProcess, xmlFile) { }

		private IEnumerable<HighlightingInfo> FindWordHighlightings(XmlValueToken node) {
			var absoluteUnquotedRange = node.GetTreeTextRange();

			var validRange = FindTrueDocumentRange(new TreeTextRange(
				absoluteUnquotedRange.StartOffset + node.UnquotedValueRange.StartOffset,
				node.UnquotedValueRange.Length
			));
			if (!validRange.HasValue) return Enumerable.Empty<HighlightingInfo>();

			return FindWordHighlightings(node, node.UnquotedValue, validRange.Value);
		}

		protected override SpellingErrorHighlightingBase CreateHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions) {
			return new XmlSpellingErrorHighlighting(node, errorRange, wordInError, getSuggestions);
		}

		public override void Execute(Action<DaemonStageResult> committer) {
			var highlightings = new List<HighlightingInfo>(0);

			CodeFile.ProcessChildren<XmlFloatingTextToken>(node => {
				if(!node.IsIdentifier() && !node.IsWhitespaceToken())
					highlightings.AddRange(FindWordHighlightings(node));
			});

			CodeFile.ProcessChildren<XmlValueToken>(node => highlightings.AddRange(FindWordHighlightings(node)));

			CodeFile.ProcessChildren<XmlToken>(node => {
				if (node.XmlTokenTypes.COMMENT_BODY.Equals(node.NodeType))
					highlightings.AddRange(FindWordHighlightings(node));
			});

			committer(new DaemonStageResult(highlightings));
		}

		

	}
}
