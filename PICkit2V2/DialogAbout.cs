using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PICkit2V2
{
    public partial class DialogAbout : Form
    {
        public DialogAbout()
        {
            InitializeComponent();
            displayAppVer.Text =  Constants.AppVersion;
            displayDevFileVer.Text = PICkitFunctions.DeviceFileVersion;
            displayPk2FWVer.Text = PICkitFunctions.FirmwareVersion;
            textBox1.Select(0,0);
			if (!PICkitFunctions.isPK3)
			{
				label2.Text = "2-";
			}
        }

        private void ClickOK(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MicrochipLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitMicrochipSite();
            }
            catch
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }

        }

        private void VisitMicrochipSite()
        {
            // Change the color of the link text by setting LinkVisited 
            // to true.
            linkLabel1.LinkVisited = true;
            //Call the Process.Start method to open the default browser 
            //with a URL:
            System.Diagnostics.Process.Start("http://www.microchip.com");

        }
        private void KairusLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitKairusSite();
            }
            catch
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }

        }

        private void VisitKairusSite()
        {
            // Change the color of the link text by setting LinkVisited 
            // to true.
            linkLabel2.LinkVisited = true;
            //Call the Process.Start method to open the default browser 
            //with a URL:
            System.Diagnostics.Process.Start("http://kair.us/projects/pickit2minus/");

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}