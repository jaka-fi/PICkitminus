using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pk2 = PICkit2V2.PICkitFunctions;

namespace PICkit2V2
{
    public partial class DialogVDDErase : Form
    {
        public DialogVDDErase()
        {
            InitializeComponent();
            
        }
        
        public void UpdateText()
        {
            label2.Text = "This device requires a minimum VDD of " 
                    + Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase.ToString() 
                    + "V\nfor Bulk Erase operations.";      
        }

        private void continueClick(object sender, EventArgs e)
        {
            if (checkBoxDoNotShow.Checked)
            {
                FormPICkit2.ShowWriteEraseVDDDialog = false;
            }
            FormPICkit2.ContinueWriteErase = true;
            this.Close();
        }

        private void cancelClick(object sender, EventArgs e)
        {
            FormPICkit2.ContinueWriteErase = false;
            this.Close();
        }
    }
}