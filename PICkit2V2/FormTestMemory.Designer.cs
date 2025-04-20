namespace PICkit2V2
{
    partial class FormTestMemory
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridTestMemory = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTestMemSize = new System.Windows.Forms.TextBox();
            this.labelTestMemSize8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxTestMemImportExport = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelBLConfig = new System.Windows.Forms.Label();
            this.textBoxBaselineConfig = new System.Windows.Forms.TextBox();
            this.labelNotSupported = new System.Windows.Forms.Label();
            this.buttonClearTestMem = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonWriteCalWords = new System.Windows.Forms.Button();
            this.labelCalWarning = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTestMemory)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridTestMemory
            // 
            this.dataGridTestMemory.AllowUserToAddRows = false;
            this.dataGridTestMemory.AllowUserToDeleteRows = false;
            this.dataGridTestMemory.AllowUserToResizeColumns = false;
            this.dataGridTestMemory.AllowUserToResizeRows = false;
            this.dataGridTestMemory.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridTestMemory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridTestMemory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridTestMemory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridTestMemory.ColumnHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridTestMemory.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridTestMemory.Location = new System.Drawing.Point(16, 52);
            this.dataGridTestMemory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridTestMemory.MultiSelect = false;
            this.dataGridTestMemory.Name = "dataGridTestMemory";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridTestMemory.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridTestMemory.RowHeadersVisible = false;
            this.dataGridTestMemory.RowHeadersWidth = 75;
            this.dataGridTestMemory.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridTestMemory.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridTestMemory.RowTemplate.Height = 17;
            this.dataGridTestMemory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridTestMemory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridTestMemory.Size = new System.Drawing.Size(467, 171);
            this.dataGridTestMemory.TabIndex = 5;
            this.dataGridTestMemory.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridTestMemory_CellEndEdit);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Test Memory Words:";
            // 
            // textBoxTestMemSize
            // 
            this.textBoxTestMemSize.Location = new System.Drawing.Point(160, 20);
            this.textBoxTestMemSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxTestMemSize.Name = "textBoxTestMemSize";
            this.textBoxTestMemSize.Size = new System.Drawing.Size(64, 22);
            this.textBoxTestMemSize.TabIndex = 7;
            this.textBoxTestMemSize.Leave += new System.EventHandler(this.textBoxTestMemSize_Leave);
            this.textBoxTestMemSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxTestMemSize_KeyPress);
            // 
            // labelTestMemSize8
            // 
            this.labelTestMemSize8.AutoSize = true;
            this.labelTestMemSize8.ForeColor = System.Drawing.Color.Red;
            this.labelTestMemSize8.Location = new System.Drawing.Point(233, 16);
            this.labelTestMemSize8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTestMemSize8.Name = "labelTestMemSize8";
            this.labelTestMemSize8.Size = new System.Drawing.Size(92, 34);
            this.labelTestMemSize8.TabIndex = 22;
            this.labelTestMemSize8.Text = "Must be\r\nmultiple of 16";
            this.labelTestMemSize8.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 226);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(120, 17);
            this.label11.TabIndex = 23;
            this.label11.Text = "Hex Import-Export";
            // 
            // checkBoxTestMemImportExport
            // 
            this.checkBoxTestMemImportExport.AutoSize = true;
            this.checkBoxTestMemImportExport.Location = new System.Drawing.Point(16, 246);
            this.checkBoxTestMemImportExport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxTestMemImportExport.Name = "checkBoxTestMemImportExport";
            this.checkBoxTestMemImportExport.Size = new System.Drawing.Size(161, 21);
            this.checkBoxTestMemImportExport.TabIndex = 24;
            this.checkBoxTestMemImportExport.Text = "Include Test Memory";
            this.checkBoxTestMemImportExport.UseVisualStyleBackColor = true;
            this.checkBoxTestMemImportExport.CheckedChanged += new System.EventHandler(this.checkBoxTestMemImportExport_CheckedChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.label2.Location = new System.Drawing.Point(357, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 25;
            this.label2.Text = "UserIDs";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.LightSalmon;
            this.label3.Location = new System.Drawing.Point(423, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 26;
            this.label3.Text = "Config";
            // 
            // labelBLConfig
            // 
            this.labelBLConfig.AutoSize = true;
            this.labelBLConfig.Location = new System.Drawing.Point(307, 246);
            this.labelBLConfig.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBLConfig.Name = "labelBLConfig";
            this.labelBLConfig.Size = new System.Drawing.Size(110, 17);
            this.labelBLConfig.TabIndex = 27;
            this.labelBLConfig.Text = "Baseline Config:";
            // 
            // textBoxBaselineConfig
            // 
            this.textBoxBaselineConfig.BackColor = System.Drawing.Color.LightSalmon;
            this.textBoxBaselineConfig.Location = new System.Drawing.Point(417, 242);
            this.textBoxBaselineConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxBaselineConfig.Name = "textBoxBaselineConfig";
            this.textBoxBaselineConfig.Size = new System.Drawing.Size(64, 22);
            this.textBoxBaselineConfig.TabIndex = 28;
            this.textBoxBaselineConfig.Text = "0000";
            this.textBoxBaselineConfig.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxBaselineConfig.Leave += new System.EventHandler(this.textBoxBaselineConfig_Leave);
            this.textBoxBaselineConfig.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxBaselineConfig_KeyPress);
            // 
            // labelNotSupported
            // 
            this.labelNotSupported.AutoSize = true;
            this.labelNotSupported.BackColor = System.Drawing.SystemColors.Info;
            this.labelNotSupported.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotSupported.ForeColor = System.Drawing.Color.Red;
            this.labelNotSupported.Location = new System.Drawing.Point(37, 100);
            this.labelNotSupported.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelNotSupported.Name = "labelNotSupported";
            this.labelNotSupported.Size = new System.Drawing.Size(379, 62);
            this.labelNotSupported.TabIndex = 29;
            this.labelNotSupported.Text = "Test Memory Not Supported\r\n            on this family";
            this.labelNotSupported.Visible = false;
            // 
            // buttonClearTestMem
            // 
            this.buttonClearTestMem.Location = new System.Drawing.Point(208, 239);
            this.buttonClearTestMem.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonClearTestMem.Name = "buttonClearTestMem";
            this.buttonClearTestMem.Size = new System.Drawing.Size(67, 28);
            this.buttonClearTestMem.TabIndex = 30;
            this.buttonClearTestMem.Text = "Clear";
            this.buttonClearTestMem.UseVisualStyleBackColor = true;
            this.buttonClearTestMem.Click += new System.EventHandler(this.buttonClearTestMem_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Gold;
            this.label4.Location = new System.Drawing.Point(423, 28);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 16);
            this.label4.TabIndex = 31;
            this.label4.Text = "CalWrd";
            // 
            // buttonWriteCalWords
            // 
            this.buttonWriteCalWords.Location = new System.Drawing.Point(335, 266);
            this.buttonWriteCalWords.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonWriteCalWords.Name = "buttonWriteCalWords";
            this.buttonWriteCalWords.Size = new System.Drawing.Size(111, 28);
            this.buttonWriteCalWords.TabIndex = 32;
            this.buttonWriteCalWords.Text = "Write CalWrd";
            this.buttonWriteCalWords.UseVisualStyleBackColor = true;
            this.buttonWriteCalWords.Click += new System.EventHandler(this.buttonWriteCalWords_Click);
            // 
            // labelCalWarning
            // 
            this.labelCalWarning.AutoSize = true;
            this.labelCalWarning.ForeColor = System.Drawing.Color.Red;
            this.labelCalWarning.Location = new System.Drawing.Point(323, 226);
            this.labelCalWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCalWarning.Name = "labelCalWarning";
            this.labelCalWarning.Size = new System.Drawing.Size(136, 34);
            this.labelCalWarning.TabIndex = 33;
            this.labelCalWarning.Text = "Writing Cal Words\r\nwill erase ALL Flash!";
            this.labelCalWarning.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelCalWarning.Visible = false;
            // 
            // FormTestMemory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Controls.Add(this.labelCalWarning);
            this.Controls.Add(this.buttonWriteCalWords);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonClearTestMem);
            this.Controls.Add(this.labelNotSupported);
            this.Controls.Add(this.textBoxBaselineConfig);
            this.Controls.Add(this.labelBLConfig);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxTestMemImportExport);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.labelTestMemSize8);
            this.Controls.Add(this.textBoxTestMemSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridTestMemory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTestMemory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test memory";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTestMemory_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTestMemory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridTestMemory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTestMemSize;
        private System.Windows.Forms.Label labelTestMemSize8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBoxTestMemImportExport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelBLConfig;
        private System.Windows.Forms.TextBox textBoxBaselineConfig;
        private System.Windows.Forms.Label labelNotSupported;
        private System.Windows.Forms.Button buttonClearTestMem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonWriteCalWords;
        private System.Windows.Forms.Label labelCalWarning;
    }
}