namespace YouCantSpell.ReSharper.CSharp
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Application.Settings;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Feature.Services.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.Util;

  /// <summary>
  /// This is the core process responsible for locating spelling mistakes within a C# source file.
  /// </summary>
  public class CSharpSpellCheckDaemonStageProcess : CStyleSpellCheckDaemonStageProcessBase<ICSharpFile>
  {
    public CSharpSpellCheckDaemonStageProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, ICSharpFile cSharpFile)
      : base(process, settingsStore, cSharpFile, CSharpLanguage.Instance)
    {
    }

    protected override SpellingErrorHighlightingBase CreateErrorHighlighting(ITreeNode node, DocumentRange errorRange, string wordInError, Func<string, string[]> getSuggestions)
    {
      return new CSharpSpellingErrorHighlighting(node, errorRange, wordInError, getSuggestions);
    }

    protected override bool IsIgnored(string text)
    {
      return base.IsIgnored(text) || CSharpUtil.IsKeyword(text);
    }

    protected override IEnumerable<IType> GetRelatedTypes(IDeclaration declaration)
    {
      if (declaration is ITypeDeclaration)
      {
        return this.GetSuperTypes(declaration as ITypeDeclaration).Cast<IType>();
      }

      var fieldDeclaration = declaration as IFieldDeclaration;
      if (fieldDeclaration != null)
      {
        return new[] { fieldDeclaration.Type };
      }

      var parameterDeclaration = declaration as IParameterDeclaration;
      if (parameterDeclaration != null)
      {
        return new[] { parameterDeclaration.Type };
      }

      var propertyDeclaration = declaration as IPropertyDeclaration;
      if (propertyDeclaration != null)
      {
        return new[] { propertyDeclaration.Type };
      }

      var variableDeclaration = declaration as ILocalVariableDeclaration;
      if (variableDeclaration != null)
      {
        return new[] { variableDeclaration.Type };
      }

      var methodDeclaration = declaration as IMethodDeclaration;
      if (methodDeclaration != null)
      {
        return new List<IType>(methodDeclaration.ParameterDeclarations.Select(x => x.Type)) { methodDeclaration.Type };
      }

      return Enumerable.Empty<IType>();
    }

    public override void Execute(Action<DaemonStageResult> committer)
    {
      var localIdentifierNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
      var highlightings = new List<HighlightingInfo>();

      this.CodeFile.ProcessDescendants(new RecursiveElementProcessor<IIdentifier>(node =>
      {
        localIdentifierNames.Add(node.Name);
        localIdentifierNames.Add(this.RemovePrefixAndSuffix(node));
        if (this.IsValidIdentifierForSpellCheck(node.Parent))
        {
          highlightings.AddRange(this.FindHighlightings(node));
        }
      }));

      var declarationsCache = this.CodeFile.GetPsiServices().Symbols;

      this.CodeFile.ProcessDescendants(new RecursiveElementProcessor<ITreeNode>(node =>
      {
        if (node is ICSharpLiteralExpression && (node as ICSharpLiteralExpression).ConstantValue.IsString())
        {
          highlightings.AddRange(this.FindStringHighlightings(node as IExpression, localIdentifierNames, declarationsCache));
        }
        else if (node is IComment)
        {
          highlightings.AddRange(this.FindHighlightings(node as IComment, localIdentifierNames, declarationsCache));
        }
      }));

      committer(new DaemonStageResult(highlightings));
    }


    public override bool IsValidIdentifierForSpellCheck(ITreeNode node)
    {
      if (null == node)
      {
        return false;
      }

      if (node is IConstructorDeclaration || node is IDestructorDeclaration)
      {
        return false; // constructor/finalizer naming is based on the class name so it would be best to flag the error there
      }

      if (node is ITypeMemberDeclaration)
      {
        // if the member inherits from above, the spelling error would be above as we have no choice but to use that name
        var declaredElement = (node as IDeclaration).DeclaredElement as IOverridableMember;
        if (null != declaredElement)
        {
          var supers = declaredElement.GetImmediateSuperMembers();
          return null != supers && supers.IsEmpty();
        }
      }

      return node is ITypeDeclaration || node is ITypeParameterDeclaration || node is ILocalVariableDeclaration || node is ITypeMemberDeclaration || node is IVariableDeclaration || node is INamespaceDeclaration;
    }
  }
}
