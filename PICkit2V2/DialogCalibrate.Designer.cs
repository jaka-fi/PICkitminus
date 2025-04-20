namespace PICkit2V2
{
    partial class DialogCalibrate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogCalibrate));
            this.panelIntro = new System.Windows.Forms.Panel();
            this.buttonClearUnitID = new System.Windows.Forms.Button();
            this.buttonClearCal = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panelSetup = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panelCal = new System.Windows.Forms.Panel();
            this.labelBadCal = new System.Windows.Forms.Label();
            this.labelGoodCal = new System.Windows.Forms.Label();
            this.buttonCalibrate = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxVDD = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panelUnitID = new System.Windows.Forms.Panel();
            this.buttonClearUnitID_PK3 = new System.Windows.Forms.Button();
            this.labelAssignedID = new System.Windows.Forms.Label();
            this.buttonSetUnitID = new System.Windows.Forms.Button();
            this.textBoxUnitID = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.panelIntro.SuspendLayout();
            this.panelSetup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelCal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panelUnitID.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIntro
            // 
            this.panelIntro.Controls.Add(this.buttonClearUnitID);
            this.panelIntro.Controls.Add(this.buttonClearCal);
            this.panelIntro.Controls.Add(this.label1);
            this.panelIntro.Controls.Add(this.label2);
            this.panelIntro.Location = new System.Drawing.Point(13, 12);
            this.panelIntro.Name = "panelIntro";
            this.panelIntro.Size = new System.Drawing.Size(330, 236);
            this.panelIntro.TabIndex = 0;
            // 
            // buttonClearUnitID
            // 
            this.buttonClearUnitID.Enabled = false;
            this.buttonClearUnitID.Location = new System.Drawing.Point(176, 205);
            this.buttonClearUnitID.Name = "buttonClearUnitID";
            this.buttonClearUnitID.Size = new System.Drawing.Size(134, 22);
            this.buttonClearUnitID.TabIndex = 2;
            this.buttonClearUnitID.Text = "Clear Unit ID";
            this.buttonClearUnitID.UseVisualStyleBackColor = true;
            this.buttonClearUnitID.Click += new System.EventHandler(this.buttonClearUnitID_Click);
            // 
            // buttonClearCal
            // 
            this.buttonClearCal.Enabled = false;
            this.buttonClearCal.Location = new System.Drawing.Point(22, 205);
            this.buttonClearCal.Name = "buttonClearCal";
            this.buttonClearCal.Size = new System.Drawing.Size(134, 22);
            this.buttonClearCal.TabIndex = 1;
            this.buttonClearCal.Text = "Clear Calibration";
            this.buttonClearCal.UseVisualStyleBackColor = true;
            this.buttonClearCal.Click += new System.EventHandler(this.buttonClearCal_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(63, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "PICkit 2 VDD Calibration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(307, 169);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // buttonBack
            // 
            this.buttonBack.Enabled = false;
            this.buttonBack.Location = new System.Drawing.Point(53, 271);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(80, 22);
            this.buttonBack.TabIndex = 1;
            this.buttonBack.Text = "< Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(140, 271);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(80, 22);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "Next >";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(226, 271);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 22);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panelSetup
            // 
            this.panelSetup.Controls.Add(this.label11);
            this.panelSetup.Controls.Add(this.label6);
            this.panelSetup.Controls.Add(this.label5);
            this.panelSetup.Controls.Add(this.label4);
            this.panelSetup.Controls.Add(this.pictureBox1);
            this.panelSetup.Controls.Add(this.label3);
            this.panelSetup.Location = new System.Drawing.Point(13, 12);
            this.panelSetup.Name = "panelSetup";
            this.panelSetup.Size = new System.Drawing.Size(330, 236);
            this.panelSetup.TabIndex = 4;
            this.panelSetup.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(4, 222);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(306, 15);
            this.label11.TabIndex = 6;
            this.label11.Text = "CAUTION: Clicking NEXT will erase existing calibration.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(4, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(289, 45);
            this.label6.TabIndex = 5;
            this.label6.Text = "Step 3:\r\nClick NEXT and the PICkit 2 will apply approximately\r\n4 Volts to the VDD" +
    " pin.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(102, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(195, 60);
            this.label5.TabIndex = 4;
            this.label5.Text = "Step 2:\r\nConnect a voltage meter between\r\npin 2 (VDD) and pin 3 (GND) of the\r\nPIC" +
    "kit 2 ICSP connector.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(102, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(186, 60);
            this.label4.TabIndex = 3;
            this.label4.Text = "Step 1:\r\nMake sure the PICkit 2 is not\r\nconnected to any device or circuit\r\nboard" +
    ".";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(78, 116);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(61, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(193, 19);
            this.label3.TabIndex = 1;
            this.label3.Text = "PICkit 2 VDD Calibration";
            // 
            // panelCal
            // 
            this.panelCal.Controls.Add(this.labelBadCal);
            this.panelCal.Controls.Add(this.labelGoodCal);
            this.panelCal.Controls.Add(this.buttonCalibrate);
            this.panelCal.Controls.Add(this.label8);
            this.panelCal.Controls.Add(this.label7);
            this.panelCal.Controls.Add(this.textBoxVDD);
            this.panelCal.Controls.Add(this.label9);
            this.panelCal.Controls.Add(this.pictureBox2);
            this.panelCal.Controls.Add(this.label10);
            this.panelCal.Location = new System.Drawing.Point(13, 12);
            this.panelCal.Name = "panelCal";
            this.panelCal.Size = new System.Drawing.Size(330, 236);
            this.panelCal.TabIndex = 6;
            this.panelCal.Visible = false;
            // 
            // labelBadCal
            // 
            this.labelBadCal.AutoSize = true;
            this.labelBadCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBadCal.ForeColor = System.Drawing.Color.Red;
            this.labelBadCal.Location = new System.Drawing.Point(3, 198);
            this.labelBadCal.Name = "labelBadCal";
            this.labelBadCal.Size = new System.Drawing.Size(276, 30);
            this.labelBadCal.TabIndex = 9;
            this.labelBadCal.Text = "Could not fully calibrate the unit.  The USB voltage\r\nmay be too low to completel" +
    "y calibrate.";
            this.labelBadCal.Visible = false;
            // 
            // labelGoodCal
            // 
            this.labelGoodCal.AutoSize = true;
            this.labelGoodCal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGoodCal.ForeColor = System.Drawing.Color.Blue;
            this.labelGoodCal.Location = new System.Drawing.Point(3, 208);
            this.labelGoodCal.Name = "labelGoodCal";
            this.labelGoodCal.Size = new System.Drawing.Size(170, 15);
            this.labelGoodCal.TabIndex = 8;
            this.labelGoodCal.Text = "CALIBRATION SUCCESSFUL!";
            this.labelGoodCal.Visible = false;
            // 
            // buttonCalibrate
            // 
            this.buttonCalibrate.Location = new System.Drawing.Point(6, 172);
            this.buttonCalibrate.Name = "buttonCalibrate";
            this.buttonCalibrate.Size = new System.Drawing.Size(112, 22);
            this.buttonCalibrate.TabIndex = 7;
            this.buttonCalibrate.Text = "Calibrate";
            this.buttonCalibrate.UseVisualStyleBackColor = true;
            this.buttonCalibrate.Click += new System.EventHandler(this.buttonCalibrate_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(287, 30);
            this.label8.TabIndex = 6;
            this.label8.Text = "Step 5:\r\nClick the CALIBRATE button to calibrate the PICkit 2.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(164, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Volts Measured";
            // 
            // textBoxVDD
            // 
            this.textBoxVDD.Location = new System.Drawing.Point(105, 102);
            this.textBoxVDD.Name = "textBoxVDD";
            this.textBoxVDD.Size = new System.Drawing.Size(53, 20);
            this.textBoxVDD.TabIndex = 4;
            this.textBoxVDD.Text = "4.000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(102, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(209, 60);
            this.label9.TabIndex = 3;
            this.label9.Text = "Step 4:\r\nEnter the actual voltage measured\r\non the volt meter in the box below, u" +
    "p\r\nto 3 decimal places.";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 22);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(78, 116);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(61, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(193, 19);
            this.label10.TabIndex = 1;
            this.label10.Text = "PICkit 2 VDD Calibration";
            // 
            // panelUnitID
            // 
            this.panelUnitID.Controls.Add(this.buttonClearUnitID_PK3);
            this.panelUnitID.Controls.Add(this.labelAssignedID);
            this.panelUnitID.Controls.Add(this.buttonSetUnitID);
            this.panelUnitID.Controls.Add(this.textBoxUnitID);
            this.panelUnitID.Controls.Add(this.label12);
            this.panelUnitID.Controls.Add(this.label16);
            this.panelUnitID.Controls.Add(this.label15);
            this.panelUnitID.Location = new System.Drawing.Point(13, 12);
            this.panelUnitID.Name = "panelUnitID";
            this.panelUnitID.Size = new System.Drawing.Size(330, 236);
            this.panelUnitID.TabIndex = 7;
            this.panelUnitID.Visible = false;
            // 
            // buttonClearUnitID_PK3
            // 
            this.buttonClearUnitID_PK3.Enabled = false;
            this.buttonClearUnitID_PK3.Location = new System.Drawing.Point(186, 188);
            this.buttonClearUnitID_PK3.Name = "buttonClearUnitID_PK3";
            this.buttonClearUnitID_PK3.Size = new System.Drawing.Size(106, 22);
            this.buttonClearUnitID_PK3.TabIndex = 8;
            this.buttonClearUnitID_PK3.Text = "Clear Unit ID";
            this.buttonClearUnitID_PK3.UseVisualStyleBackColor = true;
            this.buttonClearUnitID_PK3.Visible = false;
            this.buttonClearUnitID_PK3.Click += new System.EventHandler(this.buttonClearUnitID_PK3_Click);
            // 
            // labelAssignedID
            // 
            this.labelAssignedID.AutoSize = true;
            this.labelAssignedID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAssignedID.ForeColor = System.Drawing.Color.Blue;
            this.labelAssignedID.Location = new System.Drawing.Point(58, 217);
            this.labelAssignedID.Name = "labelAssignedID";
            this.labelAssignedID.Size = new System.Drawing.Size(179, 15);
            this.labelAssignedID.TabIndex = 7;
            this.labelAssignedID.Text = "Unit ID Assigned to this PICkit 2.";
            this.labelAssignedID.Visible = false;
            // 
            // buttonSetUnitID
            // 
            this.buttonSetUnitID.Location = new System.Drawing.Point(186, 188);
            this.buttonSetUnitID.Name = "buttonSetUnitID";
            this.buttonSetUnitID.Size = new System.Drawing.Size(106, 22);
            this.buttonSetUnitID.TabIndex = 6;
            this.buttonSetUnitID.Text = "Assign Unit ID";
            this.buttonSetUnitID.UseVisualStyleBackColor = true;
            this.buttonSetUnitID.Click += new System.EventHandler(this.buttonSetUnitID_Click);
            // 
            // textBoxUnitID
            // 
            this.textBoxUnitID.Location = new System.Drawing.Point(26, 190);
            this.textBoxUnitID.Name = "textBoxUnitID";
            this.textBoxUnitID.Size = new System.Drawing.Size(134, 20);
            this.textBoxUnitID.TabIndex = 5;
            this.textBoxUnitID.Text = "AAAAAAAAAAAAAA";
            this.textBoxUnitID.TextChanged += new System.EventHandler(this.textBoxUnitID_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(118, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 18);
            this.label12.TabIndex = 4;
            this.label12.Text = "(Optional)";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(61, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(190, 19);
            this.label16.TabIndex = 1;
            this.label16.Text = "Unit Identification Name";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(3, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(308, 120);
            this.label15.TabIndex = 3;
            this.label15.Text = resources.GetString("label15.Text");
            // 
            // DialogCalibrate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(354, 305);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.panelUnitID);
            this.Controls.Add(this.panelCal);
            this.Controls.Add(this.panelSetup);
            this.Controls.Add(this.panelIntro);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogCalibrate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DialogCalibrate";
            this.Shown += new System.EventHandler(this.DialogCalibrate_Shown);
            this.panelIntro.ResumeLayout(false);
            this.panelIntro.PerformLayout();
            this.panelSetup.ResumeLayout(false);
            this.panelSetup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelCal.ResumeLayout(false);
            this.panelCal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panelUnitID.ResumeLayout(false);
            this.panelUnitID.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelIntro;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonClearUnitID;
        private System.Windows.Forms.Button buttonClearCal;
        private System.Windows.Forms.Panel panelSetup;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panelCal;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxVDD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelBadCal;
        private System.Windows.Forms.Label labelGoodCal;
        private System.Windows.Forms.Button buttonCalibrate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panelUnitID;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button buttonSetUnitID;
        private System.Windows.Forms.TextBox textBoxUnitID;
        private System.Windows.Forms.Label labelAssignedID;
        private System.Windows.Forms.Button buttonClearUnitID_PK3;
    }
}