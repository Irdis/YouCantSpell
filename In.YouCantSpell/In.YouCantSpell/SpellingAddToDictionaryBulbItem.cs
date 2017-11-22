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
	/// A bulb item that allows adding the word to the dictionary.
	/// </summary>
	public class SpellingAddToDictionaryBulbItem :
#if RSHARP6
		JetBrains.ReSharper.Feature.Services.Bulbs.IBulbItem
#else
		IBulbAction
#endif
	{

		private readonly SpellingErrorHighlightingBase _highlight;
		private readonly string _word;

		/// <summary>
		/// Creates a new add to dictionary bulb item.
		/// </summary>
		/// <param name="highlight">The highlight that the bulb item is bound to.</param>
		public SpellingAddToDictionaryBulbItem(SpellingErrorHighlightingBase highlight) {
			if(null == highlight) throw new ArgumentNullException();
			_highlight = highlight;
			
			var word = highlight.WordInError;
			if(String.IsNullOrEmpty(word)) throw new ArgumentException("word");
			_word = word;
		}

		/// <summary>
		/// Adds the word to the dictionary.
		/// </summary>
		/// <param name="solution">The solution that is active.</param>
		/// <param name="textControl">The text control that is currently in use.</param>
		public void Execute(ISolution solution, ITextControl textControl) {
			var settingsStore = solution.GetComponent<ISettingsStore>();
			var boundSettings = settingsStore.BindToContextTransient(ContextRange.Smart((lifetime,contexts) => contexts.Empty));
			boundSettings.SetIndexedValue<SpellCheckSettings, string, byte>(x => x.UserEntries, _word, 0);
			Shell.Instance.GetComponent<ShellSpellCheckResources>().NotifyNewUserDictionaryWord(_word); // we must also notify the active spell checker of the new ignored word
            DaemonBase.GetInstance(solution).ForceReHighlight(_highlight.Range.Document); // without changing the document this is a way to request a re-processing of the document
		}

		/// <inheritdoc/>
		public string Text {
			get {
				return String.Format(
					ResourceAccessor.GetString("RS_AddToDictionaryText")
						?? @"Add ""{0}"" to dictionary",
					_word
				);
			}
		}
	}
}
