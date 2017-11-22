using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using YouCantSpell.ReSharper;

// this defines the common category information for the spelling highlighting, for example when finding problems with the solution
[assembly: RegisterConfigurableHighlightingsGroup("Spelling", "Spelling")]
[assembly: RegisterConfigurableSeverity(
	SpellingErrorHighlightingBase.SeverityId,
	null,
	"Spelling",
	"Spelling mistakes",
	"You can't spell",
	Severity.HINT
)]

namespace YouCantSpell.ReSharper
{
	/// <summary>
	/// The base spelling error highlighting for multiple code languages.
	/// </summary>
	public abstract class SpellingErrorHighlightingBase : IHighlighting
	{

		/// <summary>
		/// The core severity ID which identifies the fix highlight type. Subclasses must be attributed with this ID.
		/// </summary>
		internal const string SeverityId = "SpellingError";

		private readonly ITreeNode _node;
		private readonly string _wordInError;
		private readonly Func<string,string[]> _getSuggestions;
		private readonly DocumentRange _range;

		/// <summary>
		/// Creates a spelling highlighting for the given node and locations with the word in error as well as fix suggestions.
		/// </summary>
		/// <param name="node">The element that the highlight is bound to.</param>
		/// <param name="range">The specific document range that the highlight is bound to.</param>
		/// <param name="wordInError">The word that is in error.</param>
		/// <param name="getSuggestions">The suggestion generator that can be used to generate suggestions.</param>
		protected SpellingErrorHighlightingBase(ITreeNode node, DocumentRange range, string wordInError, Func<string, string[]> getSuggestions) {
			_node = node;
			_wordInError = wordInError;
			_range = range;
			_getSuggestions = getSuggestions;
		}

		/// <summary>
		/// The document range of the spelling error.
		/// </summary>
		public DocumentRange Range {
			get { return _range; }
		}

		/// <summary>
		/// The word that may not be spelled correctly.
		/// </summary>
		public string WordInError {
			get { return _wordInError; }
		}

		/// <summary>
		/// The suggestions for the word.
		/// </summary>
		public ReadOnlyCollection<string> Suggestions {
			get
			{
				var suggestions = (_getSuggestions ?? ShellSpellCheckResources.GetSpellingSuggestions)(WordInError)
					?? new string[0];
				// use the LINQ OrderBy as it is a stable sort.
				suggestions = suggestions
					.OrderByDescending(x => StringComparer.InvariantCultureIgnoreCase.Equals(WordInError, x))
					.ToArray();
				return Array.AsReadOnly(suggestions);
			}
		}

		/// <summary>
		/// The PSI node that contains the spelling error.
		/// </summary>
		public ITreeNode Node {
			get { return _node; }
		}

	    public DocumentRange CalculateRange()
	    {
	        return _range;
	    }

	    /// <summary>
		/// The tool-tip text that appears when the mouse hovers over the highlight.
		/// </summary>
		public string ToolTip {
			get {
				return String.Format(
					ResourceAccessor.GetString("RS_SpellingErrorTip")
						?? @"Spelling error: {0}",
					_wordInError
				);
			}
		}

		/// <inheritdoc/>
		public string ErrorStripeToolTip {
			get { return ToolTip; }
		}

		/// <inheritdoc/>
		public int NavigationOffsetPatch {
			get { return 0; }
		}

		/// <inheritdoc/>
		public bool IsValid() {
			return _node != null && _node.IsValid();
		}

	}
}
