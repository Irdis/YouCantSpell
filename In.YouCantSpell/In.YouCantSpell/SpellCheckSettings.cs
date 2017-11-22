using JetBrains.Application.Settings;
using JetBrains.Application.Settings.WellKnownRootKeys;

namespace YouCantSpell.ReSharper
{
	/// <summary>
	/// The core spell check settings.
	/// </summary>
	[SettingsKey(typeof(EnvironmentSettings), "Spell Check Settings")]
	public class SpellCheckSettings
	{

		// you need to use collections to store any kind of list in R#
		// so here I have a look-up keyed on the thing I care about and with a value that is garbage, so it is a byte
		// Is it a 1? Is it a 0? It is certainly less than 9000. Who cares, don’t use it!

		[SettingsIndexedEntryAttribute("Ignore Words")]
		public IIndexedEntry<string, byte> IgnoreEntries { get; set; }
		[SettingsIndexedEntryAttribute("User Words")]
		public IIndexedEntry<string, byte> UserEntries { get; set; }
		[SettingsIndexedEntryAttribute("Ignore Dictionaries")]
		public IIndexedEntry<string, byte> IgnoreDictionaries { get; set; }
		[SettingsIndexedEntryAttribute("Check Dictionaries")]
		public IIndexedEntry<string, byte> SpellCheckDictionaries { get; set; } 
	}
}
