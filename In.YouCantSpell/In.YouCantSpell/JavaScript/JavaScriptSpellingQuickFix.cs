using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace YouCantSpell.ReSharper.JavaScript
{

	[QuickFix]
	public class JavaScriptSpellingQuickFix : SpellingQuickFixBase<JavaScriptSpellingErrorHighlighting, JavaScriptSpellingFixBulbItem>
	{

		public JavaScriptSpellingQuickFix([NotNull] JavaScriptSpellingErrorHighlighting highlighting)
			: base(highlighting) { }

		protected override JavaScriptSpellingFixBulbItem CreateSpellingFix(string suggestion) {
			return new JavaScriptSpellingFixBulbItem(Highlighting, suggestion);
		}

	}
}
