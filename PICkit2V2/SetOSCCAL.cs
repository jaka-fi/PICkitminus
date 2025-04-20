using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;
using UTIL = PICkit2V2.Utilities;

namespace PICkit2V2
{
    public partial class SetOSCCAL : Form
    {
        public SetOSCCAL()
        {
            InitializeComponent();
            textBoxOSCCAL.Text = string.Format("{0:X4}", Pk2.DeviceBuffers.OSCCAL);
            textBoxOSCCAL.SelectAll();
        }

        private void clickSet(object sender, EventArgs e)
        {
            string editText;
            
            try
            {
                if (textBoxOSCCAL.Text.Substring(0,2) == "0x")
                {
                    editText = textBoxOSCCAL.Text;  
                }
                else if (textBoxOSCCAL.Text.Substring(0,1) == "x")
                {
                    editText = "0" + textBoxOSCCAL.Text;
                }
                else
                {    
                    editText = "0x" + textBoxOSCCAL.Text;
                }
                int value = UTIL.Convert_Value_To_Int(editText);
            
                Pk2.DeviceBuffers.OSCCAL = (uint)value;
                FormPICkit2.setOSCCALValue = true;
                this.Close();
            }
            catch
            {
                textBoxOSCCAL.Text = string.Format("{0:X4}", Pk2.DeviceBuffers.OSCCAL);
            }
            
        }

        private void clickCancel(object sender, EventArgs e)
        {
            FormPICkit2.setOSCCALValue = false;
            this.Close();
        }

    }
}