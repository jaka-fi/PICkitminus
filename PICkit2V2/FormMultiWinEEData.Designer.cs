namespace PICkit2V2
{
    partial class FormMultiWinEEData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultiWinEEData));
            this.comboBoxProgMemView = new System.Windows.Forms.ComboBox();
            this.dataGridProgramMemory = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemContextSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemContextCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.displayEEProgInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridProgramMemory)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxProgMemView
            // 
            this.comboBoxProgMemView.BackColor = System.Drawing.SystemColors.Info;
            this.comboBoxProgMemView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProgMemView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxProgMemView.FormattingEnabled = true;
            this.comboBoxProgMemView.Items.AddRange(new object[] {
            "Hex Only",
            "Word ASCII",
            "Byte ASCII"});
            this.comboBoxProgMemView.Location = new System.Drawing.Point(12, 11);
            this.comboBoxProgMemView.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxProgMemView.Name = "comboBoxProgMemView";
            this.comboBoxProgMemView.Size = new System.Drawing.Size(91, 21);
            this.comboBoxProgMemView.TabIndex = 8;
            this.comboBoxProgMemView.SelectionChangeCommitted += new System.EventHandler(this.comboBoxProgMemView_SelectionChangeCommitted);
            // 
            // dataGridProgramMemory
            // 
            this.dataGridProgramMemory.AllowUserToAddRows = false;
            this.dataGridProgramMemory.AllowUserToDeleteRows = false;
            this.dataGridProgramMemory.AllowUserToResizeColumns = false;
            this.dataGridProgramMemory.AllowUserToResizeRows = false;
            this.dataGridProgramMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridProgramMemory.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridProgramMemory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridProgramMemory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridProgramMemory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridProgramMemory.ColumnHeadersVisible = false;
            this.dataGridProgramMemory.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridProgramMemory.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridProgramMemory.Enabled = false;
            this.dataGridProgramMemory.Location = new System.Drawing.Point(12, 39);
            this.dataGridProgramMemory.Name = "dataGridProgramMemory";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridProgramMemory.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridProgramMemory.RowHeadersVisible = false;
            this.dataGridProgramMemory.RowHeadersWidth = 75;
            this.dataGridProgramMemory.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridProgramMemory.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridProgramMemory.RowTemplate.Height = 17;
            this.dataGridProgramMemory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridProgramMemory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridProgramMemory.Size = new System.Drawing.Size(510, 49);
            this.dataGridProgramMemory.TabIndex = 7;
            this.dataGridProgramMemory.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.progMemEdit);
            this.dataGridProgramMemory.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridProgramMemory_CellMouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemContextSelectAll,
            this.toolStripMenuItemContextCopy});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(164, 48);
            // 
            // toolStripMenuItemContextSelectAll
            // 
            this.toolStripMenuItemContextSelectAll.Name = "toolStripMenuItemContextSelectAll";
            this.toolStripMenuItemContextSelectAll.ShortcutKeyDisplayString = "Ctrl-A";
            this.toolStripMenuItemContextSelectAll.Size = new System.Drawing.Size(163, 22);
            this.toolStripMenuItemContextSelectAll.Text = "Select All";
            this.toolStripMenuItemContextSelectAll.Click += new System.EventHandler(this.toolStripMenuItemContextSelectAll_Click);
            // 
            // toolStripMenuItemContextCopy
            // 
            this.toolStripMenuItemContextCopy.Name = "toolStripMenuItemContextCopy";
            this.toolStripMenuItemContextCopy.ShortcutKeyDisplayString = "Ctrl-C";
            this.toolStripMenuItemContextCopy.Size = new System.Drawing.Size(163, 22);
            this.toolStripMenuItemContextCopy.Text = "Copy";
            this.toolStripMenuItemContextCopy.Click += new System.EventHandler(this.toolStripMenuItemContextCopy_Click);
            // 
            // displayEEProgInfo
            // 
            this.displayEEProgInfo.AutoSize = true;
            this.displayEEProgInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayEEProgInfo.ForeColor = System.Drawing.Color.Red;
            this.displayEEProgInfo.Location = new System.Drawing.Point(107, 14);
            this.displayEEProgInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.displayEEProgInfo.Name = "displayEEProgInfo";
            this.displayEEProgInfo.Size = new System.Drawing.Size(206, 13);
            this.displayEEProgInfo.TabIndex = 9;
            this.displayEEProgInfo.Text = "Preserve EEPROM and User IDs on write.";
            this.displayEEProgInfo.Visible = false;
            // 
            // FormMultiWinEEData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 100);
            this.Controls.Add(this.displayEEProgInfo);
            this.Controls.Add(this.comboBoxProgMemView);
            this.Controls.Add(this.dataGridProgramMemory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(200, 110);
            this.Name = "FormMultiWinEEData";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PICkit EEPROM Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMultiWinEEData_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.FormMultiWinEEData_ResizeEnd);
            this.Resize += new System.EventHandler(this.FormMultiWinEEData_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridProgramMemory)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxProgMemView;
        private System.Windows.Forms.DataGridView dataGridProgramMemory;
        private System.Windows.Forms.Label displayEEProgInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemContextSelectAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemContextCopy;
    }
}