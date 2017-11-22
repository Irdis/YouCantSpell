namespace YouCantSpell.ReSharper
{
	partial class PrimaryOptionsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing)
			{
				if (null != stringListBoxUserDictionary)
					stringListBoxUserDictionary.Dispose();
				if (null != stringListBoxIgnoredWords)
					stringListBoxIgnoredWords.Dispose();
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tableLayoutPanelUserWords = new System.Windows.Forms.TableLayoutPanel();
			this.groupBoxUserDictionary = new System.Windows.Forms.GroupBox();
			this.stringListBoxUserDictionary = new YouCantSpell.ReSharper.StringListBox();
			this.groupBoxIgnoredWords = new System.Windows.Forms.GroupBox();
			this.stringListBoxIgnoredWords = new YouCantSpell.ReSharper.StringListBox();
			this.tabControlOptions = new System.Windows.Forms.TabControl();
			this.tabPageUserWords = new System.Windows.Forms.TabPage();
			this.tabPageDict = new System.Windows.Forms.TabPage();
			this.dataGridViewDictList = new System.Windows.Forms.DataGridView();
			this.DictNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ForSpellCheckColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ForIgnoreColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.buttonOpenDictFolder = new System.Windows.Forms.Button();
			this.buttonRefreshDictList = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelPutYourDictInThatFolder = new System.Windows.Forms.Label();
			this.linkLabelOooNew = new System.Windows.Forms.LinkLabel();
			this.linkLabelOooOld = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanelUserWords.SuspendLayout();
			this.groupBoxUserDictionary.SuspendLayout();
			this.groupBoxIgnoredWords.SuspendLayout();
			this.tabControlOptions.SuspendLayout();
			this.tabPageUserWords.SuspendLayout();
			this.tabPageDict.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewDictList)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanelUserWords
			// 
			this.tableLayoutPanelUserWords.ColumnCount = 2;
			this.tableLayoutPanelUserWords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelUserWords.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelUserWords.Controls.Add(this.groupBoxUserDictionary, 0, 0);
			this.tableLayoutPanelUserWords.Controls.Add(this.groupBoxIgnoredWords, 1, 0);
			this.tableLayoutPanelUserWords.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelUserWords.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelUserWords.Name = "tableLayoutPanelUserWords";
			this.tableLayoutPanelUserWords.RowCount = 2;
			this.tableLayoutPanelUserWords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanelUserWords.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanelUserWords.Size = new System.Drawing.Size(604, 467);
			this.tableLayoutPanelUserWords.TabIndex = 0;
			// 
			// groupBoxUserDictionary
			// 
			this.groupBoxUserDictionary.Controls.Add(this.stringListBoxUserDictionary);
			this.groupBoxUserDictionary.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxUserDictionary.Location = new System.Drawing.Point(3, 3);
			this.groupBoxUserDictionary.Name = "groupBoxUserDictionary";
			this.groupBoxUserDictionary.Size = new System.Drawing.Size(296, 431);
			this.groupBoxUserDictionary.TabIndex = 0;
			this.groupBoxUserDictionary.TabStop = false;
			this.groupBoxUserDictionary.Text = "User Dictionary";
			// 
			// stringListBoxUserDictionary
			// 
			this.stringListBoxUserDictionary.CurrentItems = new string[0];
			this.stringListBoxUserDictionary.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stringListBoxUserDictionary.Location = new System.Drawing.Point(3, 16);
			this.stringListBoxUserDictionary.Name = "stringListBoxUserDictionary";
			this.stringListBoxUserDictionary.Size = new System.Drawing.Size(290, 412);
			this.stringListBoxUserDictionary.TabIndex = 0;
			// 
			// groupBoxIgnoredWords
			// 
			this.groupBoxIgnoredWords.Controls.Add(this.stringListBoxIgnoredWords);
			this.groupBoxIgnoredWords.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxIgnoredWords.Location = new System.Drawing.Point(305, 3);
			this.groupBoxIgnoredWords.Name = "groupBoxIgnoredWords";
			this.groupBoxIgnoredWords.Size = new System.Drawing.Size(296, 431);
			this.groupBoxIgnoredWords.TabIndex = 1;
			this.groupBoxIgnoredWords.TabStop = false;
			this.groupBoxIgnoredWords.Text = "Ignored Words";
			// 
			// stringListBoxIgnoredWords
			// 
			this.stringListBoxIgnoredWords.CurrentItems = new string[0];
			this.stringListBoxIgnoredWords.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stringListBoxIgnoredWords.Location = new System.Drawing.Point(3, 16);
			this.stringListBoxIgnoredWords.Name = "stringListBoxIgnoredWords";
			this.stringListBoxIgnoredWords.Size = new System.Drawing.Size(290, 412);
			this.stringListBoxIgnoredWords.TabIndex = 0;
			// 
			// tabControlOptions
			// 
			this.tabControlOptions.Controls.Add(this.tabPageUserWords);
			this.tabControlOptions.Controls.Add(this.tabPageDict);
			this.tabControlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlOptions.Location = new System.Drawing.Point(0, 0);
			this.tabControlOptions.Name = "tabControlOptions";
			this.tabControlOptions.SelectedIndex = 0;
			this.tabControlOptions.Size = new System.Drawing.Size(618, 499);
			this.tabControlOptions.TabIndex = 1;
			// 
			// tabPageUserWords
			// 
			this.tabPageUserWords.Controls.Add(this.tableLayoutPanelUserWords);
			this.tabPageUserWords.Location = new System.Drawing.Point(4, 22);
			this.tabPageUserWords.Name = "tabPageUserWords";
			this.tabPageUserWords.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUserWords.Size = new System.Drawing.Size(610, 473);
			this.tabPageUserWords.TabIndex = 0;
			this.tabPageUserWords.Text = "User Words";
			this.tabPageUserWords.UseVisualStyleBackColor = true;
			// 
			// tabPageDict
			// 
			this.tabPageDict.Controls.Add(this.dataGridViewDictList);
			this.tabPageDict.Controls.Add(this.tableLayoutPanel1);
			this.tabPageDict.Location = new System.Drawing.Point(4, 22);
			this.tabPageDict.Name = "tabPageDict";
			this.tabPageDict.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDict.Size = new System.Drawing.Size(610, 473);
			this.tabPageDict.TabIndex = 1;
			this.tabPageDict.Text = "Dictionaries";
			this.tabPageDict.ToolTipText = " ";
			this.tabPageDict.UseVisualStyleBackColor = true;
			// 
			// dataGridViewDictList
			// 
			this.dataGridViewDictList.AllowUserToAddRows = false;
			this.dataGridViewDictList.AllowUserToDeleteRows = false;
			this.dataGridViewDictList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewDictList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DictNameColumn,
            this.ForSpellCheckColumn,
            this.ForIgnoreColumn});
			this.dataGridViewDictList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewDictList.Location = new System.Drawing.Point(3, 3);
			this.dataGridViewDictList.Name = "dataGridViewDictList";
			this.dataGridViewDictList.Size = new System.Drawing.Size(604, 336);
			this.dataGridViewDictList.TabIndex = 1;
			this.dataGridViewDictList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDictList_CellValueChanged);
			this.dataGridViewDictList.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridViewDictList_CurrentCellDirtyStateChanged);
			// 
			// DictNameColumn
			// 
			this.DictNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.DictNameColumn.HeaderText = "Dictionary";
			this.DictNameColumn.Name = "DictNameColumn";
			this.DictNameColumn.ReadOnly = true;
			// 
			// ForSpellCheckColumn
			// 
			this.ForSpellCheckColumn.HeaderText = "Spell Check";
			this.ForSpellCheckColumn.Name = "ForSpellCheckColumn";
			// 
			// ForIgnoreColumn
			// 
			this.ForIgnoreColumn.HeaderText = "Ignore Words";
			this.ForIgnoreColumn.Name = "ForIgnoreColumn";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Controls.Add(this.buttonOpenDictFolder, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonRefreshDictList, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 339);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 131);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// buttonOpenDictFolder
			// 
			this.buttonOpenDictFolder.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttonOpenDictFolder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonOpenDictFolder.Location = new System.Drawing.Point(3, 3);
			this.buttonOpenDictFolder.Name = "buttonOpenDictFolder";
			this.buttonOpenDictFolder.Size = new System.Drawing.Size(195, 125);
			this.buttonOpenDictFolder.TabIndex = 0;
			this.buttonOpenDictFolder.Text = "1) Open the dictionary folder...";
			this.buttonOpenDictFolder.UseVisualStyleBackColor = true;
			this.buttonOpenDictFolder.Click += new System.EventHandler(this.buttonOpenDictFolder_Click);
			// 
			// buttonRefreshDictList
			// 
			this.buttonRefreshDictList.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttonRefreshDictList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonRefreshDictList.Location = new System.Drawing.Point(405, 3);
			this.buttonRefreshDictList.Name = "buttonRefreshDictList";
			this.buttonRefreshDictList.Size = new System.Drawing.Size(196, 125);
			this.buttonRefreshDictList.TabIndex = 1;
			this.buttonRefreshDictList.Text = "3) Refresh dictionary list!";
			this.buttonRefreshDictList.UseVisualStyleBackColor = true;
			this.buttonRefreshDictList.Click += new System.EventHandler(this.buttonRefreshDictList_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelPutYourDictInThatFolder);
			this.panel1.Controls.Add(this.linkLabelOooNew);
			this.panel1.Controls.Add(this.linkLabelOooOld);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(201, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(3);
			this.panel1.Size = new System.Drawing.Size(201, 131);
			this.panel1.TabIndex = 2;
			// 
			// labelPutYourDictInThatFolder
			// 
			this.labelPutYourDictInThatFolder.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelPutYourDictInThatFolder.Location = new System.Drawing.Point(3, 3);
			this.labelPutYourDictInThatFolder.Name = "labelPutYourDictInThatFolder";
			this.labelPutYourDictInThatFolder.Padding = new System.Windows.Forms.Padding(0, 0, 0, 13);
			this.labelPutYourDictInThatFolder.Size = new System.Drawing.Size(195, 84);
			this.labelPutYourDictInThatFolder.TabIndex = 2;
			this.labelPutYourDictInThatFolder.Text = "2) Place your dictionaries in that folder.\r\nBe sure to include both files:\r\n<name" +
    ">.dic and <name>.aff";
			this.labelPutYourDictInThatFolder.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// linkLabelOooNew
			// 
			this.linkLabelOooNew.Cursor = System.Windows.Forms.Cursors.Hand;
			this.linkLabelOooNew.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.linkLabelOooNew.Location = new System.Drawing.Point(3, 87);
			this.linkLabelOooNew.Margin = new System.Windows.Forms.Padding(3);
			this.linkLabelOooNew.Name = "linkLabelOooNew";
			this.linkLabelOooNew.Size = new System.Drawing.Size(195, 13);
			this.linkLabelOooNew.TabIndex = 3;
			this.linkLabelOooNew.TabStop = true;
			this.linkLabelOooNew.Text = "new Open Office dictionaries";
			this.linkLabelOooNew.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabelOooNew.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOooNew_LinkClicked);
			// 
			// linkLabelOooOld
			// 
			this.linkLabelOooOld.Cursor = System.Windows.Forms.Cursors.Hand;
			this.linkLabelOooOld.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.linkLabelOooOld.Location = new System.Drawing.Point(3, 100);
			this.linkLabelOooOld.Margin = new System.Windows.Forms.Padding(3);
			this.linkLabelOooOld.Name = "linkLabelOooOld";
			this.linkLabelOooOld.Size = new System.Drawing.Size(195, 13);
			this.linkLabelOooOld.TabIndex = 4;
			this.linkLabelOooOld.TabStop = true;
			this.linkLabelOooOld.Text = "old Open Office dictionaries";
			this.linkLabelOooOld.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.linkLabelOooOld.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOooOld_LinkClicked);
			// 
			// label1
			// 
			this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label1.Location = new System.Drawing.Point(3, 113);
			this.label1.Margin = new System.Windows.Forms.Padding(3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(195, 15);
			this.label1.TabIndex = 5;
			this.label1.Text = "Use 7-Zip to extract dictionaries.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// PrimaryOptionsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControlOptions);
			this.Name = "PrimaryOptionsControl";
			this.Size = new System.Drawing.Size(618, 499);
			this.Load += new System.EventHandler(this.PrimaryOptionsControl_Load);
			this.tableLayoutPanelUserWords.ResumeLayout(false);
			this.groupBoxUserDictionary.ResumeLayout(false);
			this.groupBoxIgnoredWords.ResumeLayout(false);
			this.tabControlOptions.ResumeLayout(false);
			this.tabPageUserWords.ResumeLayout(false);
			this.tabPageDict.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewDictList)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelUserWords;
		private System.Windows.Forms.GroupBox groupBoxUserDictionary;
		private System.Windows.Forms.GroupBox groupBoxIgnoredWords;
		private StringListBox stringListBoxUserDictionary;
		private StringListBox stringListBoxIgnoredWords;
		private System.Windows.Forms.TabControl tabControlOptions;
		private System.Windows.Forms.TabPage tabPageUserWords;
		private System.Windows.Forms.TabPage tabPageDict;
		private System.Windows.Forms.DataGridView dataGridViewDictList;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button buttonOpenDictFolder;
		private System.Windows.Forms.Button buttonRefreshDictList;
		private System.Windows.Forms.Label labelPutYourDictInThatFolder;
		private System.Windows.Forms.DataGridViewTextBoxColumn DictNameColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ForSpellCheckColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ForIgnoreColumn;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.LinkLabel linkLabelOooNew;
		private System.Windows.Forms.LinkLabel linkLabelOooOld;
		private System.Windows.Forms.Label label1;

	}
}
