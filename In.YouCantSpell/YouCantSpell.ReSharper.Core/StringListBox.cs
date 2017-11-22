using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace YouCantSpell.ReSharper
{
	public partial class StringListBox : UserControl
	{

		private HashSet<string> _allItemsAddedByUser;
		private HashSet<string> _allItemsRemovedByUser; 

		public StringListBox() {
			InitializeComponent();
			ResetUserItems();
		}

		public void ClearPendingItemsNoNotify()
		{
			ResetUserItems();
		}

		private void ResetUserItems() {
			_allItemsAddedByUser = new HashSet<string>();
			_allItemsRemovedByUser = new HashSet<string>();
		}

		public void SetItems(IEnumerable<string> textItems) {
			listBox.Items.Clear();
			foreach(var item in textItems.Where(x => null != x))
				listBox.Items.Add(item);
		}

		public void Initialize(IEnumerable<string> textItems)
		{
			SetItems(textItems);
			ResetUserItems();
		}

		public ReadOnlyCollection<string> ItemsToAdd {
			get {
				return new ReadOnlyCollection<string>(
					_allItemsAddedByUser
						.Where(x => !_allItemsRemovedByUser.Contains(x))
						.ToList()
				);
			}
		}

		public ReadOnlyCollection<string> ItemsToRemove {
			get {
				return new ReadOnlyCollection<string>(
					_allItemsRemovedByUser
						.Where(x => !_allItemsAddedByUser.Contains(x))
						.ToList()
				);
			}
		}

		public string[] CurrentItems {
			get { return listBox.Items.Cast<string>().ToArray(); }
			set { SetItems(value ?? Enumerable.Empty<string>()); }
		}

		public event EventHandler CurrentItemsChanged;

		private void buttonAdd_Click(object sender, EventArgs e) {
			var textToAdd = textBoxNewItem.Text;
			if(!String.IsNullOrEmpty(textToAdd) && !listBox.Items.Contains(textToAdd)) {
				listBox.Items.Add(textToAdd);
				_allItemsAddedByUser.Add(textToAdd);
				if(null != CurrentItemsChanged)
					CurrentItemsChanged(this, e);
			}
			textBoxNewItem.Text = String.Empty;
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			var selectedIndex = listBox.SelectedIndex;
			if(selectedIndex >= 0) {
				var textToRemove = listBox.SelectedItem as string;
				if(null != textToRemove) {
					listBox.Items.RemoveAt(selectedIndex);
					_allItemsRemovedByUser.Add(textToRemove);
					if(null != CurrentItemsChanged)
						CurrentItemsChanged(this, e);

					textBoxNewItem.Text = textToRemove;
				}
			}
		}

		
	}
}
