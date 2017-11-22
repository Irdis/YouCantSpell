namespace YouCantSpell.ReSharper
{
	partial class StringListBox
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
			if(disposing && (components != null)) {
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.textBoxNewItem = new System.Windows.Forms.TextBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.listBox = new System.Windows.Forms.ListBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel1.Controls.Add(this.textBoxNewItem, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonAdd, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.buttonRemove, 2, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 318);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(254, 26);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// textBoxNewItem
			// 
			this.textBoxNewItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxNewItem.Location = new System.Drawing.Point(0, 0);
			this.textBoxNewItem.Margin = new System.Windows.Forms.Padding(0);
			this.textBoxNewItem.Name = "textBoxNewItem";
			this.textBoxNewItem.Size = new System.Drawing.Size(194, 20);
			this.textBoxNewItem.TabIndex = 0;
			// 
			// buttonAdd
			// 
			this.buttonAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.buttonAdd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonAdd.Location = new System.Drawing.Point(194, 0);
			this.buttonAdd.Margin = new System.Windows.Forms.Padding(0);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(30, 26);
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "+";
			this.buttonAdd.UseVisualStyleBackColor = false;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.buttonRemove.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonRemove.Location = new System.Drawing.Point(224, 0);
			this.buttonRemove.Margin = new System.Windows.Forms.Padding(0);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(30, 26);
			this.buttonRemove.TabIndex = 2;
			this.buttonRemove.Text = "x";
			this.buttonRemove.UseVisualStyleBackColor = false;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// listBox
			// 
			this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox.FormattingEnabled = true;
			this.listBox.Location = new System.Drawing.Point(0, 0);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(254, 318);
			this.listBox.TabIndex = 1;
			// 
			// StringListBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listBox);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StringListBox";
			this.Size = new System.Drawing.Size(254, 344);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox textBoxNewItem;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.ListBox listBox;
	}
}
