namespace PICkit2V2
{
    partial class DialogDevFile
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonLoadDevFile = new System.Windows.Forms.Button();
            this.listBoxDevFiles = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a Device File to load:";
            // 
            // buttonLoadDevFile
            // 
            this.buttonLoadDevFile.Location = new System.Drawing.Point(144, 284);
            this.buttonLoadDevFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonLoadDevFile.Name = "buttonLoadDevFile";
            this.buttonLoadDevFile.Size = new System.Drawing.Size(100, 28);
            this.buttonLoadDevFile.TabIndex = 2;
            this.buttonLoadDevFile.Text = "Load";
            this.buttonLoadDevFile.UseVisualStyleBackColor = true;
            this.buttonLoadDevFile.Click += new System.EventHandler(this.buttonLoadDevFile_Click);
            // 
            // listBoxDevFiles
            // 
            this.listBoxDevFiles.FormattingEnabled = true;
            this.listBoxDevFiles.ItemHeight = 16;
            this.listBoxDevFiles.Location = new System.Drawing.Point(20, 31);
            this.listBoxDevFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBoxDevFiles.Name = "listBoxDevFiles";
            this.listBoxDevFiles.Size = new System.Drawing.Size(352, 244);
            this.listBoxDevFiles.TabIndex = 3;
            // 
            // DialogDevFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(389, 327);
            this.Controls.Add(this.listBoxDevFiles);
            this.Controls.Add(this.buttonLoadDevFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogDevFile";
            this.Text = "DialogDevFile";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonLoadDevFile;
        private System.Windows.Forms.ListBox listBoxDevFiles;

    }
}