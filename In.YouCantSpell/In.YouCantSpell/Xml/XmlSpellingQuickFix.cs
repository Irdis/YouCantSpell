using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace YouCantSpell.ReSharper.Xml
{
	[QuickFix]
	public class XmlSpellingQuickFix : SpellingQuickFixBase<XmlSpellingErrorHighlighting, XmlSpellingFixBulbItem>
	{

		public XmlSpellingQuickFix([NotNull] XmlSpellingErrorHighlighting highlighting)
			: base(highlighting) { }

		protected override XmlSpellingFixBulbItem CreateSpellingFix(string suggestion) {
			return new XmlSpellingFixBulbItem(Highlighting, suggestion);
		}
	}
}
