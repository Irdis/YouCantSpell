using System;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions.ActionManager;
using JetBrains.DataFlow;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Refactorings.Specific.Rename;
using JetBrains.ReSharper.Features.Navigation.Goto.GotoProviders;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Refactorings.Rename;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;

namespace YouCantSpell.ReSharper
{
	/// <summary>
	/// Assists in renaming a declared element.
	/// </summary>
	/// <remarks>
	/// This is inspired by the sample project because I don't know where the hell else this functionality is documented at.
	/// </remarks>
	[SolutionComponent]
	public class RenameRefactoringExecutor
	{
		private const string RootRuleName = "ManualChangeNameFix";

		private readonly ActionManager _actionManager;
		private readonly ISolution _solution;
		private readonly RenameRefactoringService _renameRefactoringService;

		public RenameRefactoringExecutor(ActionManager actionManager, ISolution solution, RenameRefactoringService renameRefactoringService) {
			_actionManager = actionManager;
			_solution = solution;
			_renameRefactoringService = renameRefactoringService;
		}

		/// <summary>
		/// Executes the rename utility on the declared element with the given new text.
		/// </summary>
		/// <param name="declaredElement">The element to be renamed.</param>
		/// <param name="textControl">The text control that is required to execute the rename.</param>
		/// <param name="newText">The new text to use.</param>
		public void Execute(IDeclaredElement declaredElement, ITextControl textControl, string newText)
		{
		    RenameRefactoringService.Rename(_solution,
		        new RenameDataProvider((IDeclaredElement)declaredElement, newText), textControl);
		}
	}
}
