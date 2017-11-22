using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace YouCantSpell.ReSharper.Html
{
	[QuickFix]
	public class HtmlSpellingQuickFix : SpellingQuickFixBase<HtmlSpellingErrorHighlighting, HtmlSpellingFixBulbItem>
	{

		public HtmlSpellingQuickFix([NotNull] HtmlSpellingErrorHighlighting highlighting)
			: base(highlighting) { }

		protected override HtmlSpellingFixBulbItem CreateSpellingFix(string suggestion) {
			return new HtmlSpellingFixBulbItem(Highlighting, suggestion);
		}
	}
}
