using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace PICkit2V2
{
    public partial class DialogDevFile : Form
    {
        public DialogDevFile()
        {
            InitializeComponent();
            
            // Find & list all the *.dat files
            System.IO.DirectoryInfo searchdir = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
            foreach (System.IO.FileInfo file in searchdir.GetFiles("*.dat"))
            {
                listBoxDevFiles.Items.Add(file.Name);
            }
        }

        private void buttonLoadDevFile_Click(object sender, EventArgs e)
        {
            FormPICkit2.DeviceFileName = listBoxDevFiles.SelectedItem.ToString();
            this.Close();
        }
    }
}