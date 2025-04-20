using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PICkit2V2
{
    public partial class DialogCustomBaud : Form
    {
        public DialogCustomBaud()
        {
            InitializeComponent();
            textBox1.Focus();
            if (PICkitFunctions.isPK2M == false && PICkitFunctions.isPK3 == false)    // PK2M and PK3 support higher baud rates, but original PK2 doesn't
            {
                maximumBaud.Text = "Maximum = 38400 baud\r\n(PK3 and PK2M can use higher)";
            }
            else
            {
                maximumBaud.Text = "Maximum = 115200 baud\r\n(short bursts up to 400000 baud)";
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                if (!char.IsDigit(textBox1.Text[textBox1.Text.Length-1]))
                {
                    textBox1.Text = textBox1.Text.Substring(0, (textBox1.Text.Length-1));
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                int baud = int.Parse(textBox1.Text);
                int maxbaud = 400000;
                if (PICkitFunctions.isPK2M == false && PICkitFunctions.isPK3 == false)    // PK2M and PK3 support higher baud rates, but original PK2 doesn't
                {
                    maxbaud = 38400;
                }
                
                if ((baud < 150) || (baud > maxbaud))
                {
                    MessageBox.Show("Baud value is outside\nthe Min / Max range.");
                }
                else
                {
                    DialogUART.CustomBaud = textBox1.Text;
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("Illegal Value.");
            }
        }
    }
}