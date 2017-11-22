using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xml;

namespace YouCantSpell.ReSharper.Xml
{
	[ConfigurableSeverityHighlighting(SeverityId, XmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE)]
	public class XmlSpellingErrorHighlighting : SpellingErrorHighlightingBase
	{

		public XmlSpellingErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions)
			: base(node, errorRange, wordInError, getSuggestions) { }

	}
}
