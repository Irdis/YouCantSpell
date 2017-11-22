using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.JavaScript.LanguageImpl;
using JetBrains.ReSharper.Psi.Tree;

namespace YouCantSpell.ReSharper.JavaScript
{
	[ConfigurableSeverityHighlighting(SeverityId, JavaScriptLanguage.Name, OverlapResolve = OverlapResolveKind.NONE)]
	public class JavaScriptSpellingErrorHighlighting : SpellingErrorHighlightingBase
	{

		public JavaScriptSpellingErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions)
			: base(node, errorRange, wordInError, getSuggestions) { }

	}
}
