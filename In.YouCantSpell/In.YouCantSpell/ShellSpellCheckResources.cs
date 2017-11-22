using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using System;
using JetBrains.ReSharper.Resources.Shell;

namespace YouCantSpell.ReSharper
{

	/// <summary>
	/// Contains spell check resources shared for the entire visual studio instance.
	/// </summary>
	[ShellComponent]
	public class ShellSpellCheckResources : IDisposable
	{

		private static WeakReference _mostRecentInstance;

		/// <summary>
		/// Returns the most recent spell check resources.
		/// </summary>
		/// <remarks>
		/// I can't figure out how to get access to this instance from the options page
		/// so this is a bit of a hack to access the most recent instance, which should
		/// also be what I would expect to get from the ReSharper IoC stuff, if I knew
		/// how to use it.
		/// </remarks>
		[Obsolete("This is a dirty filthy no good hack.")]
		public static ShellSpellCheckResources MostRecentInstance{
			get { return null == _mostRecentInstance ? null : (ShellSpellCheckResources) _mostRecentInstance.Target; }
		}

		public static string[] GetSpellingSuggestions(string word) {
			var spellChecker = Shell.Instance.GetComponent<ShellSpellCheckResources>().SpellChecker;
			return spellChecker.GetRecommendations(word);
		}

		private SpellCheckerPointer _spellChecker;
		private readonly object _spellCheckerSync = new object();

		private CachedSpellChecker _ignoreDictionaries;
		private HashSet<string> _ignored;
		private HashSet<string> _ignoredInsensitive;
		private readonly object _ignoredSync = new object();

		public ShellSpellCheckResources([NotNull] ISettingsStore settingsStore)
			: this(settingsStore.BindToContextTransient(ContextRange.Smart((lifetime, contexts) => contexts.Empty))) { }

		public ShellSpellCheckResources([NotNull] IContextBoundSettingsStore boundSettings)
		{
			FullReset(boundSettings);
			_mostRecentInstance = new WeakReference(this);
		}

		/// <summary>
		/// Resets the spell check resources to their initial states and reloads the settings that are to be applied.
		/// </summary>
		/// <param name="boundSettings">Optional settings.</param>
		/// <remarks>
		/// If settings are not provided they will be retrieved automatically.
		/// The settingsStore parameter is to be used to override the settings store that is used.
		/// </remarks>
		public void FullReset(IContextBoundSettingsStore boundSettings = null)
		{
			if (null == boundSettings){
				var settingsStore = Shell.Instance.GetComponent<ISettingsStore>();
				if (null == settingsStore)
					return;

				boundSettings = settingsStore.BindToContextTransient(ContextRange.Smart((lifetime, contexts) => contexts.Empty));
			}

			ResetSpellChecker(boundSettings);
			ResetIgnoreLists(boundSettings);
		}

		private void ResetSpellChecker(IContextBoundSettingsStore boundSettings)
		{

			var selectedSepllCheckers = new HashSet<string>(boundSettings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.SpellCheckDictionaries));
			// we sort the spell checkers so that just in case we don't have any set we can take the EN dictionaries first.
			var availableSepllCheckers = YouCantSpell.SpellChecker.GetAllAvailableDictionaryNames().OrderByDescending(x =>{
				if (String.Equals(x, "EN_US", StringComparison.OrdinalIgnoreCase))
					return 100;
				if (x.StartsWith("EN", StringComparison.OrdinalIgnoreCase))
					return 50;
				return 0;
			}).ToList();

			var activeSepllCheckers = availableSepllCheckers.Where(selectedSepllCheckers.Contains).ToList();
			if (activeSepllCheckers.Count == 0 && availableSepllCheckers.Count > 0){
				// if there are no active spell checkers we should take one off the top of the available list, preferring en_us and en_gb.
				activeSepllCheckers.Add(availableSepllCheckers[0]);
			}

			ISpellChecker newSpellCheckCore = new SpellCheckerCollection(activeSepllCheckers.Select(x => new SpellChecker(x)).Cast<ISpellChecker>());
			newSpellCheckCore.Add(boundSettings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.UserEntries));
			newSpellCheckCore = new CachedSpellChecker(newSpellCheckCore, true);

			lock (_spellCheckerSync){
				if(null == _spellChecker)
					_spellChecker = new SpellCheckerPointer(newSpellCheckCore, true);
				else
					_spellChecker.Replace(newSpellCheckCore);
			}

		}

		private void ResetIgnoreLists(IContextBoundSettingsStore boundSettings){
			CachedSpellChecker oldIgnoreDictionaries;
			var selectedIgnoreDictionaries = new HashSet<string>(boundSettings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.IgnoreDictionaries));
			var activeIgnoreDictionaries = YouCantSpell.SpellChecker.GetAllAvailableDictionaryNames().Where(selectedIgnoreDictionaries.Contains).ToList();
			ISpellChecker newIgnoreDictionaries = null;
			if (activeIgnoreDictionaries.Count > 0) {
				newIgnoreDictionaries = activeIgnoreDictionaries.Count == 1
					? (ISpellChecker)new SpellChecker(activeIgnoreDictionaries[0])
					: new SpellCheckerCollection(activeIgnoreDictionaries.Select(x => new SpellChecker(x)).Cast<ISpellChecker>());
			}

			var ignoreWords = new HashSet<string>(boundSettings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.IgnoreEntries));
			var ignoreWordsInsensitive = new HashSet<string>(ignoreWords, StringComparer.CurrentCultureIgnoreCase);
			lock (_ignoredSync) {
				oldIgnoreDictionaries = _ignoreDictionaries;
				_ignored = ignoreWords;
				_ignoredInsensitive = ignoreWordsInsensitive;
				_ignoreDictionaries = null == newIgnoreDictionaries ? null : new CachedSpellChecker(newIgnoreDictionaries, true);
			}

			if(null != oldIgnoreDictionaries)
				oldIgnoreDictionaries.Dispose();
		}

		public ISpellChecker SpellChecker { get { return _spellChecker; } }

		/// <summary>
		/// Determines if the given word is ignored using case sensitive rules.
		/// </summary>
		/// <param name="word">The word to check.</param>
		/// <returns>True if the word is ignored.</returns>
		public bool IsIgnored(string word) {
			lock(_ignoredSync) {
				return _ignored.Contains(word) || (null != _ignoreDictionaries && _ignoreDictionaries.Check(word));
			}
		}

		/// <summary>
		/// Determines if the given word is ignored using case insensitive rules.
		/// </summary>
		/// <param name="word">The word to check.</param>
		/// <returns>True if the word is ignored.</returns>
		public bool IsIgnoredInsensitive(string word) {
			lock(_ignoredSync) {
				return _ignoredInsensitive.Contains(word) || (null != _ignoreDictionaries && _ignoreDictionaries.Check(word));
			}
		}

		/// <summary>
		/// Signals to the active spell checker that there is a new word in the dictionary.
		/// </summary>
		/// <param name="word">The word to add to the dictionary.</param>
		/// <remarks>
		/// NOTE: this is a temporary addition to the dictionary. For a permanent additions you must add the word to the settings.
		/// </remarks>
		internal void NotifyNewUserDictionaryWord(string word) {
			SpellChecker.Add(word);
		}

		/// <summary>
		/// Signals to the active spell checker resources that there is a new ignored word.
		/// </summary>
		/// <param name="word">The new word to ignore.</param>
		/// <remarks>
		/// NOTE: this is a temporary addition to the ignore list. For permanent additions you must add the word to the settings.
		/// </remarks>
		internal void NotifyNewIgnoreWord(string word)
		{
			_ignored.Add(word);
			_ignoredInsensitive.Add(word);
		}

		/// <inheritdoc/>
		public void Dispose() {
			if(null != _spellChecker)
			{
				_spellChecker.Dispose();
				_spellChecker = null;
			}
		}

		~ShellSpellCheckResources()
		{
			Dispose();
		}

		
	}
}
