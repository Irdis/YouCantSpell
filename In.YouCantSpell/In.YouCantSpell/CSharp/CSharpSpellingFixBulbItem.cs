namespace YouCantSpell.ReSharper.CSharp
{
  using System;
  using System.Linq;
  using JetBrains.Application.Progress;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.Util;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using YouCantSpell.Utility;

  public class CSharpSpellingFixBulbItem : SpellingFixBulbItemBase<CSharpSpellingErrorHighlighting>
  {

    public CSharpSpellingFixBulbItem(CSharpSpellingErrorHighlighting highlighting, string suggestion)
      : base(highlighting, suggestion)
    {
    }

    protected override Action<JetBrains.TextControl.ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
      if (!this.Highlighting.IsValid())
      {
        return null;
      }

      var node = this.Highlighting.Node;
      var badWordTextRange = this.Highlighting.CalculateRange().TextRange;

      var newText = StringUtil.ReplaceSection(node.GetText(), this.Suggestion, badWordTextRange.StartOffset - node.GetDocumentRange().TextRange.StartOffset, badWordTextRange.Length);

      if (node is IIdentifier)
      {
        return textControl =>
        {
          var declaredIdentifier = TextControlToPsi.GetDeclaredElements(solution, textControl).FirstOrDefault();
          solution.GetComponent<RenameRefactoringExecutor>().Execute(declaredIdentifier, textControl, newText);
        };
      }

      if (node is IComment)
      {
        var elementFactory = CSharpElementFactory.GetInstance(node.GetPsiModule());
        var newComment = elementFactory.CreateComment(newText);
        ModificationUtil.ReplaceChild(node, newComment);
        return null;
      }

      if (node is IExpression)
      {
        var expression = node as IExpression;
        if (expression.ConstantValue.IsString())
        {
          var elementFactory = CSharpElementFactory.GetInstance(node.GetPsiModule());
          var newStringLiteral = elementFactory.CreateExpression("$0", newText);
          ModificationUtil.ReplaceChild(node, newStringLiteral);
          return null;
        }
      }

      return null;
    }
  }
}
