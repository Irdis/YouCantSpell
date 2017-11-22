using System;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace YouCantSpell.ReSharper
{
	/// <summary>
	/// A spelling bulb item that adds a word to the spelling ignore list.
	/// </summary>
	public class SpellingIgnoreWordBulbItem :
#if RSHARP6
		JetBrains.ReSharper.Feature.Services.Bulbs.IBulbItem
#else
		IBulbAction
#endif
	{

		private readonly SpellingErrorHighlightingBase _highlight;
		/// <summary>
		/// The word that could be ignored.
		/// </summary>
		private readonly string _word;

		/// <summary>
		/// Creates a new ignore bulb item that adds the word to the ignore list when executed.
		/// </summary>
		/// <param name="highlight">The highlight that the bulb item is bound to.</param>
		public SpellingIgnoreWordBulbItem(SpellingErrorHighlightingBase highlight)
		{
			if (null == highlight) throw new ArgumentNullException("highlight");
			_highlight = highlight;

			var word = highlight.WordInError;
			if (String.IsNullOrEmpty(word)) throw new ArgumentException("word");
			_word = word;
		}

		/// <summary>
		/// Adds the bound word to the ignore list.
		/// </summary>
		/// <param name="solution">The current solution.</param>
		/// <param name="textControl">The current text control.</param>
		public void Execute(ISolution solution, ITextControl textControl) {
			var settingsStore = solution.GetComponent<ISettingsStore>();
			var boundSettings = settingsStore.BindToContextTransient(ContextRange.Smart((lifetime, contexts) => contexts.Empty));
			boundSettings.SetIndexedValue<SpellCheckSettings, string, byte>(x => x.IgnoreEntries, _word, 0);
			Shell.Instance.GetComponent<ShellSpellCheckResources>().NotifyNewIgnoreWord(_word); // we must also notify the active spell checker of the new ignored word
			DaemonBase.GetInstance(solution).ForceReHighlight(_highlight.Range.Document); // without changing the document this is a way to request a re-processing of the document
		}

		/// <inheritdoc/>
		public string Text {
			get {
				return String.Format(
					ResourceAccessor.GetString("RS_AddToIgnoreList")
						?? "Ignore spelling for \"{0}\"",
					_word
				);
			}
		}
	}
}
