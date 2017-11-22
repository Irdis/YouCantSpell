namespace YouCantSpell.ReSharper.CSharp
{
  using System;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Feature.Services.Daemon;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.Tree;

  [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE)]
  public class CSharpSpellingErrorHighlighting : SpellingErrorHighlightingBase
  {
    public CSharpSpellingErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions)
      : base(node, errorRange, wordInError, getSuggestions)
    {
    }
  }
}
