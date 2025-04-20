namespace PICkit2V2
{
    partial class dialogSounds
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dialogSounds));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxSuccess = new System.Windows.Forms.CheckBox();
            this.checkBoxWarning = new System.Windows.Forms.CheckBox();
            this.checkBoxError = new System.Windows.Forms.CheckBox();
            this.textBoxSuccessFile = new System.Windows.Forms.TextBox();
            this.textBoxWarningFile = new System.Windows.Forms.TextBox();
            this.textBoxErrorFile = new System.Windows.Forms.TextBox();
            this.buttonSuccessBrowse = new System.Windows.Forms.Button();
            this.buttonWarningBrowse = new System.Windows.Forms.Button();
            this.buttonErrorBrowse = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.openFileDialogWAV = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(516, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Enable:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(110, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Play this WAV file:";
            // 
            // checkBoxSuccess
            // 
            this.checkBoxSuccess.BackColor = System.Drawing.Color.LimeGreen;
            this.checkBoxSuccess.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxSuccess.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxSuccess.Location = new System.Drawing.Point(15, 80);
            this.checkBoxSuccess.Name = "checkBoxSuccess";
            this.checkBoxSuccess.Size = new System.Drawing.Size(74, 17);
            this.checkBoxSuccess.TabIndex = 1;
            this.checkBoxSuccess.Text = "Success";
            this.checkBoxSuccess.UseVisualStyleBackColor = false;
            this.checkBoxSuccess.Click += new System.EventHandler(this.checkBoxSuccess_CheckedChanged);
            // 
            // checkBoxWarning
            // 
            this.checkBoxWarning.BackColor = System.Drawing.Color.Yellow;
            this.checkBoxWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxWarning.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxWarning.Location = new System.Drawing.Point(15, 103);
            this.checkBoxWarning.Name = "checkBoxWarning";
            this.checkBoxWarning.Size = new System.Drawing.Size(74, 17);
            this.checkBoxWarning.TabIndex = 1;
            this.checkBoxWarning.Text = "Warning";
            this.checkBoxWarning.UseVisualStyleBackColor = false;
            this.checkBoxWarning.Click += new System.EventHandler(this.checkBoxWarning_CheckedChanged);
            // 
            // checkBoxError
            // 
            this.checkBoxError.BackColor = System.Drawing.Color.Salmon;
            this.checkBoxError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxError.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxError.Location = new System.Drawing.Point(15, 126);
            this.checkBoxError.Name = "checkBoxError";
            this.checkBoxError.Size = new System.Drawing.Size(74, 17);
            this.checkBoxError.TabIndex = 1;
            this.checkBoxError.Text = "Error";
            this.checkBoxError.UseVisualStyleBackColor = false;
            this.checkBoxError.Click += new System.EventHandler(this.checkBoxError_CheckedChanged);
            // 
            // textBoxSuccessFile
            // 
            this.textBoxSuccessFile.Location = new System.Drawing.Point(113, 78);
            this.textBoxSuccessFile.Name = "textBoxSuccessFile";
            this.textBoxSuccessFile.Size = new System.Drawing.Size(341, 20);
            this.textBoxSuccessFile.TabIndex = 2;
            // 
            // textBoxWarningFile
            // 
            this.textBoxWarningFile.Location = new System.Drawing.Point(113, 101);
            this.textBoxWarningFile.Name = "textBoxWarningFile";
            this.textBoxWarningFile.Size = new System.Drawing.Size(341, 20);
            this.textBoxWarningFile.TabIndex = 2;
            // 
            // textBoxErrorFile
            // 
            this.textBoxErrorFile.Location = new System.Drawing.Point(113, 124);
            this.textBoxErrorFile.Name = "textBoxErrorFile";
            this.textBoxErrorFile.Size = new System.Drawing.Size(341, 20);
            this.textBoxErrorFile.TabIndex = 2;
            // 
            // buttonSuccessBrowse
            // 
            this.buttonSuccessBrowse.Location = new System.Drawing.Point(460, 76);
            this.buttonSuccessBrowse.Name = "buttonSuccessBrowse";
            this.buttonSuccessBrowse.Size = new System.Drawing.Size(60, 23);
            this.buttonSuccessBrowse.TabIndex = 3;
            this.buttonSuccessBrowse.Text = "Browse";
            this.buttonSuccessBrowse.UseVisualStyleBackColor = true;
            this.buttonSuccessBrowse.Click += new System.EventHandler(this.buttonSuccessBrowse_Click);
            // 
            // buttonWarningBrowse
            // 
            this.buttonWarningBrowse.Location = new System.Drawing.Point(460, 99);
            this.buttonWarningBrowse.Name = "buttonWarningBrowse";
            this.buttonWarningBrowse.Size = new System.Drawing.Size(60, 23);
            this.buttonWarningBrowse.TabIndex = 3;
            this.buttonWarningBrowse.Text = "Browse";
            this.buttonWarningBrowse.UseVisualStyleBackColor = true;
            this.buttonWarningBrowse.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonErrorBrowse
            // 
            this.buttonErrorBrowse.Location = new System.Drawing.Point(460, 122);
            this.buttonErrorBrowse.Name = "buttonErrorBrowse";
            this.buttonErrorBrowse.Size = new System.Drawing.Size(60, 23);
            this.buttonErrorBrowse.TabIndex = 3;
            this.buttonErrorBrowse.Text = "Browse";
            this.buttonErrorBrowse.UseVisualStyleBackColor = true;
            this.buttonErrorBrowse.Click += new System.EventHandler(this.buttonErrorBrowse_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(437, 151);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(83, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // openFileDialogWAV
            // 
            this.openFileDialogWAV.DefaultExt = "wav";
            this.openFileDialogWAV.Filter = "WAV files|*.wav|All files|*.*";
            this.openFileDialogWAV.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogWAV_FileOk);
            // 
            // dialogSounds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(532, 182);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonErrorBrowse);
            this.Controls.Add(this.buttonWarningBrowse);
            this.Controls.Add(this.buttonSuccessBrowse);
            this.Controls.Add(this.textBoxErrorFile);
            this.Controls.Add(this.textBoxWarningFile);
            this.Controls.Add(this.textBoxSuccessFile);
            this.Controls.Add(this.checkBoxError);
            this.Controls.Add(this.checkBoxWarning);
            this.Controls.Add(this.checkBoxSuccess);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dialogSounds";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alert Sounds";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxSuccess;
        private System.Windows.Forms.CheckBox checkBoxWarning;
        private System.Windows.Forms.CheckBox checkBoxError;
        private System.Windows.Forms.TextBox textBoxSuccessFile;
        private System.Windows.Forms.TextBox textBoxWarningFile;
        private System.Windows.Forms.TextBox textBoxErrorFile;
        private System.Windows.Forms.Button buttonSuccessBrowse;
        private System.Windows.Forms.Button buttonWarningBrowse;
        private System.Windows.Forms.Button buttonErrorBrowse;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.OpenFileDialog openFileDialogWAV;
    }
}