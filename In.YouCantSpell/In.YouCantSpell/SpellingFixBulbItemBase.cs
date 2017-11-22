using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.TextControl;

namespace YouCantSpell.ReSharper
{

	/// <summary>
	/// This is the base of a spelling fix.
	/// </summary>
	/// <typeparam name="THighlighting">The highlighting type the bulb item is bound to.</typeparam>
	public abstract class SpellingFixBulbItemBase<THighlighting> :
#if RSHARP6
		JetBrains.ReSharper.Feature.Services.Bulbs.BulbItemImpl
#else
		BulbActionBase
#endif
		where THighlighting : SpellingErrorHighlightingBase
	{

		private readonly THighlighting _highlighting;
		private readonly string _suggestion;

		/// <summary>
		/// Creates a new spelling fix bulb item.
		/// </summary>
		/// <param name="highlighting">The highlighting the quick fix is bound to.</param>
		/// <param name="suggestion">The suggested spelling of the bulb item.</param>
		protected SpellingFixBulbItemBase(THighlighting highlighting, string suggestion) {
			_highlighting = highlighting;
			_suggestion = suggestion;
		}

		/// <summary>
		/// The bound highlighting.
		/// </summary>
		public THighlighting Highlighting { get { return _highlighting; } }

		/// <summary>
		/// The spelling suggestion for the highlighted word.
		/// </summary>
		public string Suggestion { get { return _suggestion; } }

		/// <inheritdoc/>
		public override string Text {
			get {
				if(String.IsNullOrEmpty(_suggestion))
					return "ERROR";
				return String.Format(
					ResourceAccessor.GetString("RS_ReplaceWith")
						?? "Replace with: {0}",
					_suggestion
				);
			}
		}

		/// <inheritdoc/>
		protected abstract override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress);

	}
}
