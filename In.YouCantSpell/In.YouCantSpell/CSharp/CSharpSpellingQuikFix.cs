namespace YouCantSpell.ReSharper.CSharp
{
  using JetBrains.Annotations;
  using JetBrains.ReSharper.Feature.Services.QuickFixes;

  /// <summary>
  /// A C# implementation of the spelling quick fix.
  /// </summary>
  [QuickFix]
  public class CSharpSpellingQuickFix : SpellingQuickFixBase<CSharpSpellingErrorHighlighting, CSharpSpellingFixBulbItem>
  {
    public CSharpSpellingQuickFix([NotNull] CSharpSpellingErrorHighlighting highlighting)
      : base(highlighting)
    {
    }

    protected override CSharpSpellingFixBulbItem CreateSpellingFix(string suggestion)
    {
      return new CSharpSpellingFixBulbItem(this.Highlighting, suggestion);
    }
  }
}
