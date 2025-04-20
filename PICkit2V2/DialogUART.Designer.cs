namespace PICkit2V2
{
    partial class DialogUART
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogUART));
            this.textBoxDisplay = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonConnect = new System.Windows.Forms.RadioButton();
            this.radioButtonDisconnect = new System.Windows.Forms.RadioButton();
            this.comboBoxBaud = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioButtonHex = new System.Windows.Forms.RadioButton();
            this.radioButtonASCII = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxString1 = new System.Windows.Forms.TextBox();
            this.buttonString1 = new System.Windows.Forms.Button();
            this.buttonString2 = new System.Windows.Forms.Button();
            this.textBoxString2 = new System.Windows.Forms.TextBox();
            this.buttonString4 = new System.Windows.Forms.Button();
            this.buttonString3 = new System.Windows.Forms.Button();
            this.textBoxString3 = new System.Windows.Forms.TextBox();
            this.textBoxString4 = new System.Windows.Forms.TextBox();
            this.buttonLog = new System.Windows.Forms.Button();
            this.buttonClearScreen = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.checkBoxEcho = new System.Windows.Forms.CheckBox();
            this.labelMacros = new System.Windows.Forms.Label();
            this.timerPollForData = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxCRLF = new System.Windows.Forms.CheckBox();
            this.saveFileDialogLogFile = new System.Windows.Forms.SaveFileDialog();
            this.checkBoxWrap = new System.Windows.Forms.CheckBox();
            this.pictureBoxHelp = new System.Windows.Forms.PictureBox();
            this.checkBoxVDD = new System.Windows.Forms.CheckBox();
            this.panelVdd = new System.Windows.Forms.Panel();
            this.labelTypeHex = new System.Windows.Forms.Label();
            this.panelVirtControls = new System.Windows.Forms.Panel();
            this.comboBoxPortName = new System.Windows.Forms.ComboBox();
            this.radioButtonVirtDisconnect = new System.Windows.Forms.RadioButton();
            this.radioButtonVirtConnect = new System.Windows.Forms.RadioButton();
            this.radioCRLF = new System.Windows.Forms.RadioButton();
            this.radioLF = new System.Windows.Forms.RadioButton();
            this.radioCR = new System.Windows.Forms.RadioButton();
            this.panelMonitor = new System.Windows.Forms.Panel();
            this.checkBoxMonitor = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).BeginInit();
            this.panelVdd.SuspendLayout();
            this.panelVirtControls.SuspendLayout();
            this.panelMonitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDisplay
            // 
            this.textBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDisplay.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxDisplay.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBoxDisplay.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDisplay.Location = new System.Drawing.Point(13, 72);
            this.textBoxDisplay.MaxLength = 17220;
            this.textBoxDisplay.MinimumSize = new System.Drawing.Size(708, 305);
            this.textBoxDisplay.Multiline = true;
            this.textBoxDisplay.Name = "textBoxDisplay";
            this.textBoxDisplay.ReadOnly = true;
            this.textBoxDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDisplay.Size = new System.Drawing.Size(708, 305);
            this.textBoxDisplay.TabIndex = 0;
            this.textBoxDisplay.Leave += new System.EventHandler(this.textBoxDisplay_Leave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 385);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(189, 116);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 504);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Connect PICkit VDD && target VDD.";
            // 
            // radioButtonConnect
            // 
            this.radioButtonConnect.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonConnect.AutoCheck = false;
            this.radioButtonConnect.Location = new System.Drawing.Point(130, 4);
            this.radioButtonConnect.Name = "radioButtonConnect";
            this.radioButtonConnect.Size = new System.Drawing.Size(70, 22);
            this.radioButtonConnect.TabIndex = 14;
            this.radioButtonConnect.Text = "Connect";
            this.radioButtonConnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonConnect.UseVisualStyleBackColor = true;
            this.radioButtonConnect.Click += new System.EventHandler(this.radioButtonConnect_Click_1);
            // 
            // radioButtonDisconnect
            // 
            this.radioButtonDisconnect.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonDisconnect.AutoCheck = false;
            this.radioButtonDisconnect.Checked = true;
            this.radioButtonDisconnect.Location = new System.Drawing.Point(206, 4);
            this.radioButtonDisconnect.Name = "radioButtonDisconnect";
            this.radioButtonDisconnect.Size = new System.Drawing.Size(70, 22);
            this.radioButtonDisconnect.TabIndex = 15;
            this.radioButtonDisconnect.TabStop = true;
            this.radioButtonDisconnect.Text = "Disconnect";
            this.radioButtonDisconnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonDisconnect.UseVisualStyleBackColor = true;
            this.radioButtonDisconnect.Click += new System.EventHandler(this.radioButtonDisconnect_Click);
            // 
            // comboBoxBaud
            // 
            this.comboBoxBaud.BackColor = System.Drawing.SystemColors.Info;
            this.comboBoxBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBaud.FormattingEnabled = true;
            this.comboBoxBaud.Location = new System.Drawing.Point(6, 5);
            this.comboBoxBaud.MaxDropDownItems = 12;
            this.comboBoxBaud.Name = "comboBoxBaud";
            this.comboBoxBaud.Size = new System.Drawing.Size(118, 21);
            this.comboBoxBaud.TabIndex = 13;
            this.comboBoxBaud.SelectedIndexChanged += new System.EventHandler(this.comboBoxBaud_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBoxBaud);
            this.panel1.Controls.Add(this.radioButtonDisconnect);
            this.panel1.Controls.Add(this.radioButtonConnect);
            this.panel1.Location = new System.Drawing.Point(13, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(280, 30);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.radioButtonHex);
            this.panel2.Controls.Add(this.radioButtonASCII);
            this.panel2.Location = new System.Drawing.Point(618, 9);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(106, 29);
            this.panel2.TabIndex = 7;
            // 
            // radioButtonHex
            // 
            this.radioButtonHex.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonHex.Location = new System.Drawing.Point(57, 3);
            this.radioButtonHex.Name = "radioButtonHex";
            this.radioButtonHex.Size = new System.Drawing.Size(47, 22);
            this.radioButtonHex.TabIndex = 17;
            this.radioButtonHex.Text = "Hex";
            this.radioButtonHex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonHex.UseVisualStyleBackColor = true;
            this.radioButtonHex.CheckedChanged += new System.EventHandler(this.radioButtonHex_CheckedChanged);
            // 
            // radioButtonASCII
            // 
            this.radioButtonASCII.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonASCII.Checked = true;
            this.radioButtonASCII.Location = new System.Drawing.Point(3, 3);
            this.radioButtonASCII.Name = "radioButtonASCII";
            this.radioButtonASCII.Size = new System.Drawing.Size(47, 22);
            this.radioButtonASCII.TabIndex = 16;
            this.radioButtonASCII.TabStop = true;
            this.radioButtonASCII.Text = "ASCII";
            this.radioButtonASCII.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonASCII.UseVisualStyleBackColor = true;
            this.radioButtonASCII.CheckedChanged += new System.EventHandler(this.radioButtonASCII_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(570, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Mode:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(542, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "8 data bits - No parity - 1 Stop bit.";
            // 
            // textBoxString1
            // 
            this.textBoxString1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxString1.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxString1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxString1.Location = new System.Drawing.Point(306, 406);
            this.textBoxString1.Name = "textBoxString1";
            this.textBoxString1.Size = new System.Drawing.Size(286, 20);
            this.textBoxString1.TabIndex = 1;
            this.textBoxString1.TextChanged += new System.EventHandler(this.textBoxString1_TextChanged);
            // 
            // buttonString1
            // 
            this.buttonString1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonString1.Enabled = false;
            this.buttonString1.Location = new System.Drawing.Point(246, 404);
            this.buttonString1.Name = "buttonString1";
            this.buttonString1.Size = new System.Drawing.Size(54, 22);
            this.buttonString1.TabIndex = 5;
            this.buttonString1.Text = "Send";
            this.buttonString1.UseVisualStyleBackColor = true;
            this.buttonString1.Click += new System.EventHandler(this.buttonString1_Click);
            // 
            // buttonString2
            // 
            this.buttonString2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonString2.Enabled = false;
            this.buttonString2.Location = new System.Drawing.Point(246, 434);
            this.buttonString2.Name = "buttonString2";
            this.buttonString2.Size = new System.Drawing.Size(54, 22);
            this.buttonString2.TabIndex = 6;
            this.buttonString2.Text = "Send";
            this.buttonString2.UseVisualStyleBackColor = true;
            this.buttonString2.Click += new System.EventHandler(this.buttonString2_Click);
            // 
            // textBoxString2
            // 
            this.textBoxString2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxString2.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxString2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxString2.Location = new System.Drawing.Point(306, 435);
            this.textBoxString2.Name = "textBoxString2";
            this.textBoxString2.Size = new System.Drawing.Size(286, 20);
            this.textBoxString2.TabIndex = 2;
            this.textBoxString2.TextChanged += new System.EventHandler(this.textBoxString2_TextChanged);
            // 
            // buttonString4
            // 
            this.buttonString4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonString4.Enabled = false;
            this.buttonString4.Location = new System.Drawing.Point(246, 490);
            this.buttonString4.Name = "buttonString4";
            this.buttonString4.Size = new System.Drawing.Size(54, 22);
            this.buttonString4.TabIndex = 8;
            this.buttonString4.Text = "Send";
            this.buttonString4.UseVisualStyleBackColor = true;
            this.buttonString4.Click += new System.EventHandler(this.buttonString4_Click);
            // 
            // buttonString3
            // 
            this.buttonString3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonString3.Enabled = false;
            this.buttonString3.Location = new System.Drawing.Point(246, 462);
            this.buttonString3.Name = "buttonString3";
            this.buttonString3.Size = new System.Drawing.Size(54, 22);
            this.buttonString3.TabIndex = 7;
            this.buttonString3.Text = "Send";
            this.buttonString3.UseVisualStyleBackColor = true;
            this.buttonString3.Click += new System.EventHandler(this.buttonString3_Click);
            // 
            // textBoxString3
            // 
            this.textBoxString3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxString3.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxString3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxString3.Location = new System.Drawing.Point(306, 464);
            this.textBoxString3.Name = "textBoxString3";
            this.textBoxString3.Size = new System.Drawing.Size(286, 20);
            this.textBoxString3.TabIndex = 3;
            this.textBoxString3.TextChanged += new System.EventHandler(this.textBoxString3_TextChanged);
            // 
            // textBoxString4
            // 
            this.textBoxString4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxString4.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxString4.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxString4.Location = new System.Drawing.Point(306, 492);
            this.textBoxString4.Name = "textBoxString4";
            this.textBoxString4.Size = new System.Drawing.Size(286, 20);
            this.textBoxString4.TabIndex = 4;
            this.textBoxString4.TextChanged += new System.EventHandler(this.textBoxString4_TextChanged);
            // 
            // buttonLog
            // 
            this.buttonLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLog.Location = new System.Drawing.Point(618, 404);
            this.buttonLog.Name = "buttonLog";
            this.buttonLog.Size = new System.Drawing.Size(102, 22);
            this.buttonLog.TabIndex = 9;
            this.buttonLog.Text = "Log to File";
            this.buttonLog.UseVisualStyleBackColor = true;
            this.buttonLog.Click += new System.EventHandler(this.buttonLog_Click);
            // 
            // buttonClearScreen
            // 
            this.buttonClearScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearScreen.Location = new System.Drawing.Point(618, 434);
            this.buttonClearScreen.Name = "buttonClearScreen";
            this.buttonClearScreen.Size = new System.Drawing.Size(102, 22);
            this.buttonClearScreen.TabIndex = 10;
            this.buttonClearScreen.Text = "Clear Screen";
            this.buttonClearScreen.UseVisualStyleBackColor = true;
            this.buttonClearScreen.Click += new System.EventHandler(this.buttonClearScreen_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.Location = new System.Drawing.Point(618, 490);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(102, 22);
            this.buttonExit.TabIndex = 12;
            this.buttonExit.Text = "Exit UART Tool";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // checkBoxEcho
            // 
            this.checkBoxEcho.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxEcho.AutoSize = true;
            this.checkBoxEcho.Checked = true;
            this.checkBoxEcho.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEcho.Location = new System.Drawing.Point(638, 466);
            this.checkBoxEcho.Name = "checkBoxEcho";
            this.checkBoxEcho.Size = new System.Drawing.Size(68, 17);
            this.checkBoxEcho.TabIndex = 11;
            this.checkBoxEcho.Text = "Echo On";
            this.checkBoxEcho.UseVisualStyleBackColor = true;
            // 
            // labelMacros
            // 
            this.labelMacros.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMacros.AutoSize = true;
            this.labelMacros.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMacros.Location = new System.Drawing.Point(243, 382);
            this.labelMacros.Name = "labelMacros";
            this.labelMacros.Size = new System.Drawing.Size(100, 15);
            this.labelMacros.TabIndex = 22;
            this.labelMacros.Text = "String Macros:";
            // 
            // timerPollForData
            // 
            this.timerPollForData.Interval = 15;
            this.timerPollForData.Tick += new System.EventHandler(this.timerPollForData_Tick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(380, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "ASCII newline:";
            // 
            // checkBoxCRLF
            // 
            this.checkBoxCRLF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxCRLF.AutoSize = true;
            this.checkBoxCRLF.Checked = true;
            this.checkBoxCRLF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCRLF.Location = new System.Drawing.Point(365, 383);
            this.checkBoxCRLF.Name = "checkBoxCRLF";
            this.checkBoxCRLF.Size = new System.Drawing.Size(157, 17);
            this.checkBoxCRLF.TabIndex = 18;
            this.checkBoxCRLF.Text = "Append CR+LF (x0D + x0A)";
            this.checkBoxCRLF.UseVisualStyleBackColor = true;
            // 
            // saveFileDialogLogFile
            // 
            this.saveFileDialogLogFile.DefaultExt = "txt";
            this.saveFileDialogLogFile.Filter = "All files|*.*|Text files|*.txt";
            this.saveFileDialogLogFile.InitialDirectory = "c:\\";
            this.saveFileDialogLogFile.Title = "Log UART data to file";
            this.saveFileDialogLogFile.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialogLogFile_FileOk);
            // 
            // checkBoxWrap
            // 
            this.checkBoxWrap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxWrap.AutoSize = true;
            this.checkBoxWrap.Checked = true;
            this.checkBoxWrap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWrap.Location = new System.Drawing.Point(638, 383);
            this.checkBoxWrap.Name = "checkBoxWrap";
            this.checkBoxWrap.Size = new System.Drawing.Size(76, 17);
            this.checkBoxWrap.TabIndex = 24;
            this.checkBoxWrap.Text = "Wrap Text";
            this.checkBoxWrap.UseVisualStyleBackColor = true;
            this.checkBoxWrap.CheckedChanged += new System.EventHandler(this.checkBoxWrap_CheckedChanged);
            // 
            // pictureBoxHelp
            // 
            this.pictureBoxHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxHelp.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxHelp.Image")));
            this.pictureBoxHelp.Location = new System.Drawing.Point(540, 10);
            this.pictureBoxHelp.Name = "pictureBoxHelp";
            this.pictureBoxHelp.Size = new System.Drawing.Size(24, 24);
            this.pictureBoxHelp.TabIndex = 26;
            this.pictureBoxHelp.TabStop = false;
            this.pictureBoxHelp.Click += new System.EventHandler(this.pictureBoxHelp_Click);
            // 
            // checkBoxVDD
            // 
            this.checkBoxVDD.AutoSize = true;
            this.checkBoxVDD.Location = new System.Drawing.Point(6, 5);
            this.checkBoxVDD.Name = "checkBoxVDD";
            this.checkBoxVDD.Size = new System.Drawing.Size(49, 17);
            this.checkBoxVDD.TabIndex = 27;
            this.checkBoxVDD.Text = "VDD";
            this.checkBoxVDD.UseVisualStyleBackColor = true;
            this.checkBoxVDD.Click += new System.EventHandler(this.checkBoxVDD_Click);
            // 
            // panelVdd
            // 
            this.panelVdd.Controls.Add(this.checkBoxVDD);
            this.panelVdd.Location = new System.Drawing.Point(299, 10);
            this.panelVdd.Name = "panelVdd";
            this.panelVdd.Size = new System.Drawing.Size(75, 27);
            this.panelVdd.TabIndex = 28;
            // 
            // labelTypeHex
            // 
            this.labelTypeHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTypeHex.AutoSize = true;
            this.labelTypeHex.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.labelTypeHex.Location = new System.Drawing.Point(520, 384);
            this.labelTypeHex.Name = "labelTypeHex";
            this.labelTypeHex.Size = new System.Drawing.Size(75, 13);
            this.labelTypeHex.TabIndex = 29;
            this.labelTypeHex.Text = "Type Hex : A_";
            this.labelTypeHex.Visible = false;
            // 
            // panelVirtControls
            // 
            this.panelVirtControls.Controls.Add(this.comboBoxPortName);
            this.panelVirtControls.Controls.Add(this.radioButtonVirtDisconnect);
            this.panelVirtControls.Controls.Add(this.radioButtonVirtConnect);
            this.panelVirtControls.Location = new System.Drawing.Point(13, 40);
            this.panelVirtControls.Name = "panelVirtControls";
            this.panelVirtControls.Size = new System.Drawing.Size(280, 30);
            this.panelVirtControls.TabIndex = 30;
            // 
            // comboBoxPortName
            // 
            this.comboBoxPortName.BackColor = System.Drawing.SystemColors.Info;
            this.comboBoxPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPortName.Enabled = false;
            this.comboBoxPortName.FormattingEnabled = true;
            this.comboBoxPortName.Location = new System.Drawing.Point(6, 5);
            this.comboBoxPortName.MaxDropDownItems = 12;
            this.comboBoxPortName.Name = "comboBoxPortName";
            this.comboBoxPortName.Size = new System.Drawing.Size(118, 21);
            this.comboBoxPortName.TabIndex = 13;
            this.comboBoxPortName.SelectedIndexChanged += new System.EventHandler(this.comboBoxPortName_SelectedIndexChanged);
            // 
            // radioButtonVirtDisconnect
            // 
            this.radioButtonVirtDisconnect.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonVirtDisconnect.AutoCheck = false;
            this.radioButtonVirtDisconnect.Checked = true;
            this.radioButtonVirtDisconnect.Enabled = false;
            this.radioButtonVirtDisconnect.Location = new System.Drawing.Point(206, 4);
            this.radioButtonVirtDisconnect.Name = "radioButtonVirtDisconnect";
            this.radioButtonVirtDisconnect.Size = new System.Drawing.Size(70, 22);
            this.radioButtonVirtDisconnect.TabIndex = 15;
            this.radioButtonVirtDisconnect.TabStop = true;
            this.radioButtonVirtDisconnect.Text = "Disconnect";
            this.radioButtonVirtDisconnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonVirtDisconnect.UseVisualStyleBackColor = true;
            this.radioButtonVirtDisconnect.Click += new System.EventHandler(this.radioButtonVirtDisconnect_Click_1);
            // 
            // radioButtonVirtConnect
            // 
            this.radioButtonVirtConnect.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonVirtConnect.AutoCheck = false;
            this.radioButtonVirtConnect.Enabled = false;
            this.radioButtonVirtConnect.Location = new System.Drawing.Point(130, 4);
            this.radioButtonVirtConnect.Name = "radioButtonVirtConnect";
            this.radioButtonVirtConnect.Size = new System.Drawing.Size(70, 22);
            this.radioButtonVirtConnect.TabIndex = 14;
            this.radioButtonVirtConnect.Text = "Connect";
            this.radioButtonVirtConnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonVirtConnect.UseVisualStyleBackColor = true;
            this.radioButtonVirtConnect.Click += new System.EventHandler(this.radioButtonVirtConnect_Click_1);
            // 
            // radioCRLF
            // 
            this.radioCRLF.AutoSize = true;
            this.radioCRLF.Checked = true;
            this.radioCRLF.Location = new System.Drawing.Point(380, 20);
            this.radioCRLF.Name = "radioCRLF";
            this.radioCRLF.Size = new System.Drawing.Size(86, 17);
            this.radioCRLF.TabIndex = 31;
            this.radioCRLF.TabStop = true;
            this.radioCRLF.Text = "CR+LF (Win)";
            this.radioCRLF.UseVisualStyleBackColor = true;
            this.radioCRLF.Click += new System.EventHandler(this.radioCRLF_Click_1);
            // 
            // radioLF
            // 
            this.radioLF.AutoSize = true;
            this.radioLF.Location = new System.Drawing.Point(380, 37);
            this.radioLF.Name = "radioLF";
            this.radioLF.Size = new System.Drawing.Size(67, 17);
            this.radioLF.TabIndex = 32;
            this.radioLF.TabStop = true;
            this.radioLF.Text = "LF (Unix)";
            this.radioLF.UseVisualStyleBackColor = true;
            this.radioLF.Click += new System.EventHandler(this.radioCRLF_Click_1);
            // 
            // radioCR
            // 
            this.radioCR.AutoSize = true;
            this.radioCR.Location = new System.Drawing.Point(380, 54);
            this.radioCR.Name = "radioCR";
            this.radioCR.Size = new System.Drawing.Size(70, 17);
            this.radioCR.TabIndex = 33;
            this.radioCR.TabStop = true;
            this.radioCR.Text = "CR (Mac)";
            this.radioCR.UseVisualStyleBackColor = true;
            this.radioCR.Click += new System.EventHandler(this.radioCRLF_Click_1);
            // 
            // panelMonitor
            // 
            this.panelMonitor.Controls.Add(this.checkBoxMonitor);
            this.panelMonitor.Location = new System.Drawing.Point(299, 41);
            this.panelMonitor.Name = "panelMonitor";
            this.panelMonitor.Size = new System.Drawing.Size(75, 27);
            this.panelMonitor.TabIndex = 34;
            // 
            // checkBoxMonitor
            // 
            this.checkBoxMonitor.AutoSize = true;
            this.checkBoxMonitor.Enabled = false;
            this.checkBoxMonitor.Location = new System.Drawing.Point(6, 5);
            this.checkBoxMonitor.Name = "checkBoxMonitor";
            this.checkBoxMonitor.Size = new System.Drawing.Size(61, 17);
            this.checkBoxMonitor.TabIndex = 27;
            this.checkBoxMonitor.Text = "Monitor";
            this.checkBoxMonitor.UseVisualStyleBackColor = true;
            // 
            // DialogUART
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(736, 532);
            this.Controls.Add(this.panelMonitor);
            this.Controls.Add(this.radioCR);
            this.Controls.Add(this.radioLF);
            this.Controls.Add(this.radioCRLF);
            this.Controls.Add(this.panelVirtControls);
            this.Controls.Add(this.labelTypeHex);
            this.Controls.Add(this.panelVdd);
            this.Controls.Add(this.pictureBoxHelp);
            this.Controls.Add(this.checkBoxWrap);
            this.Controls.Add(this.checkBoxCRLF);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelMacros);
            this.Controls.Add(this.checkBoxEcho);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonClearScreen);
            this.Controls.Add(this.buttonLog);
            this.Controls.Add(this.textBoxString4);
            this.Controls.Add(this.textBoxString3);
            this.Controls.Add(this.buttonString3);
            this.Controls.Add(this.buttonString4);
            this.Controls.Add(this.textBoxString2);
            this.Controls.Add(this.buttonString2);
            this.Controls.Add(this.buttonString1);
            this.Controls.Add(this.textBoxString1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBoxDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(744, 559);
            this.Name = "DialogUART";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PICkit 2 UART Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogUART_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHelp)).EndInit();
            this.panelVdd.ResumeLayout(false);
            this.panelVdd.PerformLayout();
            this.panelVirtControls.ResumeLayout(false);
            this.panelMonitor.ResumeLayout(false);
            this.panelMonitor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDisplay;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonConnect;
        private System.Windows.Forms.RadioButton radioButtonDisconnect;
        private System.Windows.Forms.ComboBox comboBoxBaud;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioButtonHex;
        private System.Windows.Forms.RadioButton radioButtonASCII;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxString1;
        private System.Windows.Forms.Button buttonString1;
        private System.Windows.Forms.Button buttonString2;
        private System.Windows.Forms.TextBox textBoxString2;
        private System.Windows.Forms.Button buttonString4;
        private System.Windows.Forms.Button buttonString3;
        private System.Windows.Forms.TextBox textBoxString3;
        private System.Windows.Forms.TextBox textBoxString4;
        private System.Windows.Forms.Button buttonLog;
        private System.Windows.Forms.Button buttonClearScreen;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.CheckBox checkBoxEcho;
        private System.Windows.Forms.Label labelMacros;
        private System.Windows.Forms.Timer timerPollForData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBoxCRLF;
        private System.Windows.Forms.SaveFileDialog saveFileDialogLogFile;
        private System.Windows.Forms.CheckBox checkBoxWrap;
        private System.Windows.Forms.PictureBox pictureBoxHelp;
        private System.Windows.Forms.CheckBox checkBoxVDD;
        private System.Windows.Forms.Panel panelVdd;
        private System.Windows.Forms.Label labelTypeHex;
        private System.Windows.Forms.Panel panelVirtControls;
        private System.Windows.Forms.ComboBox comboBoxPortName;
        private System.Windows.Forms.RadioButton radioButtonVirtDisconnect;
        private System.Windows.Forms.RadioButton radioButtonVirtConnect;
        private System.Windows.Forms.RadioButton radioCRLF;
        private System.Windows.Forms.RadioButton radioLF;
        private System.Windows.Forms.RadioButton radioCR;
        private System.Windows.Forms.Panel panelMonitor;
        private System.Windows.Forms.CheckBox checkBoxMonitor;
    }
}