using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Html;
using JetBrains.ReSharper.Psi.Tree;

namespace YouCantSpell.ReSharper.Html
{
	[ConfigurableSeverityHighlighting(SeverityId, HtmlLanguage.Name, OverlapResolve = OverlapResolveKind.NONE)]
	public class HtmlSpellingErrorHighlighting : SpellingErrorHighlightingBase
	{

		public HtmlSpellingErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions)
			: base(node, errorRange, wordInError, getSuggestions) { }

	}
}
