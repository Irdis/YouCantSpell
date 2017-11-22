using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.UIAutomation;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionPages;

namespace YouCantSpell.ReSharper
{

	/// <summary>
	/// The spell check options pane that is displayed in the ReSharper options UI.
	/// </summary>
#if RSHARP6
	[OptionsPage(Pid, "You Can't Spell Settings", "YouCantSpell.ReSharper.resources.SpellCheckOptionsIcon.png", ParentId = EnvironmentPage.Pid, Sequence = 100)]
#else
	[OptionsPage(Pid, "You Can't Spell Settings", typeof(BulbThemedIcons.OptionsVS), ParentId = EnvironmentPage.Pid, Sequence = 100)]
#endif

	public partial class PrimaryOptionsControl : UserControl, IOptionsPage
	{

		private const string Pid = "SpellCheckOptions";
		
		private bool _changesMade = false;
		private OptionsSettingsSmartContext _settings;

		public PrimaryOptionsControl([NotNull] Lifetime lifetime, OptionsSettingsSmartContext settings){
			_settings = settings;

			InitializeComponent();

			groupBoxUserDictionary.Text = ResourceAccessor.GetString("UI_UserDictionaryListTitle")
				?? "User Dictionary";
			groupBoxIgnoredWords.Text = ResourceAccessor.GetString("UI_IgnoredWordListTitle")
				?? "Ignored Words";

			// Extract the string keys from the settings and apply them to the UI list boxes.
			UserWords.Initialize(
				settings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.UserEntries)
				.ToArray()
			);

			IgnoreWords.Initialize(
				settings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.IgnoreEntries)
				.ToArray()
			);

			// Monitor the changed properties on the UI list boxes and when changed apply the modifications to the settings collection to be saved.

			var userWordsProperty = WinFormsProperty.Create(lifetime, UserWords, x => x.CurrentItems, true);
			userWordsProperty.Change.Advise_NoAcknowledgement(lifetime, x => ApplyChanges(settings, UserWords, p => p.UserEntries));

			var ignoreWordsProperty = WinFormsProperty.Create(lifetime, IgnoreWords, x => x.CurrentItems, true);
			ignoreWordsProperty.Change.Advise_NoAcknowledgement(lifetime, x => ApplyChanges(settings, IgnoreWords, p => p.IgnoreEntries));

			ResetDictionariesGrid(settings);
		}

		/// <summary>
		/// This method should be called when any change or changes are made to the UI word list boxes.
		/// </summary>
		/// <param name="settings">This is the settings context that we use to apply pending changes.</param>
		/// <param name="listBox">This is the list box that has all the words we added or removed.</param>
		/// <param name="property">This is the property on the settings collection that needs to be updated.</param>
		private void ApplyChanges(OptionsSettingsSmartContext settings, StringListBox listBox, Expression<Func<SpellCheckSettings, IIndexedEntry<string, byte>>> property) {
			foreach(var word in listBox.ItemsToRemove)
				settings.RemoveIndexedValue(property, word);
			foreach(var word in listBox.ItemsToAdd)
				settings.SetIndexedValue(property, word, default(byte));
			listBox.ClearPendingItemsNoNotify();
			_changesMade = true;
		}

		private void ResetDictionariesGrid(OptionsSettingsSmartContext settings = null){
			if (null == settings)
				settings = _settings;

			var availableDictionaries = SpellChecker.GetAllAvailableDictionaryNames();
			var ignoreDictionaries = new HashSet<string>(settings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.IgnoreDictionaries));
			var spellCheckDictionaries = new HashSet<string>(settings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.SpellCheckDictionaries));

			dataGridViewDictList.Rows.Clear();
			foreach (var dictionary in availableDictionaries.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)){
				var isSpellCheck = spellCheckDictionaries.Contains(dictionary);
				var isIgnore = ignoreDictionaries.Contains(dictionary);
				dataGridViewDictList.Rows.Add(dictionary, isSpellCheck, isIgnore);
			}
			dataGridViewDictList.Update();
			dataGridViewDictList.Invalidate();
		}

		public StringListBox UserWords { get { return stringListBoxUserDictionary; } }

		public StringListBox IgnoreWords { get { return stringListBoxIgnoredWords; } }

		/// <inheritdoc/>
		public EitherControl Control {
			get { return (this); }
		}

		/// <inheritdoc/>
		public string Id {
			get { return Pid; }
		}

		/// <inheritdoc/>
		public bool OnOk() {
			// When the save button is clicked we should try to signal to the core spell check resources that things have changed.
			if (_changesMade){
				var spellChecker = Shell.Instance.TryGetComponent<ShellSpellCheckResources>()
					?? ShellSpellCheckResources.MostRecentInstance;
				if (null != spellChecker)
					spellChecker.FullReset(_settings);
			}
			return true;
		}

		/// <inheritdoc/>
		public bool ValidatePage() {
			return true;
		}

		private void dataGridViewDictList_CellValueChanged(object sender, DataGridViewCellEventArgs e){
			if (e.RowIndex < 0 || e.RowIndex >= dataGridViewDictList.Rows.Count)
				return;

			var row = dataGridViewDictList.Rows[e.RowIndex];
			var dictName = Convert.ToString(row.Cells[0].Value);

			var isForSpellCheck = Convert.ToBoolean(row.Cells[1].Value);
			var isForIgnore = Convert.ToBoolean(row.Cells[2].Value);

			switch (e.ColumnIndex){
			case 0: return;
			case 1:{
				var currentForSepllCheck = new HashSet<string>(_settings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.SpellCheckDictionaries));
				if (isForSpellCheck && !currentForSepllCheck.Contains(dictName)) {
					_settings.SetIndexedValue((SpellCheckSettings x) => x.SpellCheckDictionaries, dictName, default(byte));
					_changesMade = true;
				}
				else if(!isForSpellCheck && currentForSepllCheck.Contains(dictName)) {
					_settings.RemoveIndexedValue((SpellCheckSettings x) => x.SpellCheckDictionaries, dictName);
					_changesMade = true;
				}

				if (isForSpellCheck && isForIgnore)
					row.Cells[2].Value = false; // uncheck the ignore one

				break;
			}
			case 2:{
				var currentForIgnore = new HashSet<string>(_settings.EnumEntryIndices<SpellCheckSettings, string, byte>(x => x.IgnoreDictionaries));
				if (isForIgnore && !currentForIgnore.Contains(dictName)) {
					_settings.SetIndexedValue((SpellCheckSettings x) => x.IgnoreDictionaries, dictName, default(byte));
					_changesMade = true;
				}
				else if(!isForIgnore && currentForIgnore.Contains(dictName)) {
					_settings.RemoveIndexedValue((SpellCheckSettings x) => x.IgnoreDictionaries, dictName);
					_changesMade = true;
				}

				if (isForSpellCheck && isForIgnore)
					row.Cells[1].Value = false; // uncheck the spell check one

				break;
			}
			default: return;
			}
		}

		private void dataGridViewDictList_CurrentCellDirtyStateChanged(object sender, EventArgs e){
			dataGridViewDictList.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}

		private void buttonRefreshDictList_Click(object sender, EventArgs e) {
			ResetDictionariesGrid();
		}

		private void buttonOpenDictFolder_Click(object sender, EventArgs e){
			var folderPath = SpellChecker.FindDictionaryPath();
			if (String.IsNullOrEmpty(folderPath) || !new DirectoryInfo(folderPath).Exists)
				return;

			System.Diagnostics.Process.Start(folderPath);

		}

		private void PrimaryOptionsControl_Load(object sender, EventArgs e){
		}

		private void linkLabelOooNew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("http://extensions.services.openoffice.org/dictionary");
		}

		private void linkLabelOooOld_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
			System.Diagnostics.Process.Start("http://wiki.openoffice.org/wiki/Dictionaries");
		}

		private void label1_Click(object sender, EventArgs e){
			System.Diagnostics.Process.Start("http://7-zip.org/");
		}
	}
}
