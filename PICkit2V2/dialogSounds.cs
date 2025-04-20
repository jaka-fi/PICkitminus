using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PICkit2V2
{
    public partial class dialogSounds : Form
    {
        private System.Media.SoundPlayer wavPlayer = new System.Media.SoundPlayer();
        private TextBox destSoundTextBox;
    
        public dialogSounds()
        {
            InitializeComponent();
            checkBoxSuccess.Checked = FormPICkit2.PlaySuccessWav;
            checkBoxWarning.Checked = FormPICkit2.PlayWarningWav;
            checkBoxError.Checked = FormPICkit2.PlayErrorWav;
            textBoxSuccessFile.Text = FormPICkit2.SuccessWavFile;
            textBoxWarningFile.Text = FormPICkit2.WarningWavFile;
            textBoxErrorFile.Text = FormPICkit2.ErrorWavFile;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            FormPICkit2.PlaySuccessWav = checkBoxSuccess.Checked;
            FormPICkit2.PlayWarningWav = checkBoxWarning.Checked;
            FormPICkit2.PlayErrorWav = checkBoxError.Checked;
            FormPICkit2.SuccessWavFile = textBoxSuccessFile.Text;
            FormPICkit2.WarningWavFile = textBoxWarningFile.Text;
            FormPICkit2.ErrorWavFile = textBoxErrorFile.Text;
            this.Close();
        }

        private void buttonSuccessBrowse_Click(object sender, EventArgs e)
        {
            destSoundTextBox = textBoxSuccessFile;
            openFileDialogWAV.FileName = textBoxSuccessFile.Text;
            openFileDialogWAV.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            destSoundTextBox = textBoxWarningFile;
            openFileDialogWAV.FileName = textBoxWarningFile.Text;
            openFileDialogWAV.ShowDialog();
        }

        private void buttonErrorBrowse_Click(object sender, EventArgs e)
        {
            destSoundTextBox = textBoxErrorFile;
            openFileDialogWAV.FileName = textBoxErrorFile.Text;
            openFileDialogWAV.ShowDialog();
        }

        private void checkBoxSuccess_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSuccess.Checked)
            {
                try
                {
                    wavPlayer.SoundLocation = @textBoxSuccessFile.Text;
                    wavPlayer.Play();
                }
                catch
                {
                
                }
            }
        }

        private void checkBoxWarning_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWarning.Checked)
            {
                try
                {
                    wavPlayer.SoundLocation = @textBoxWarningFile.Text;
                    wavPlayer.Play();
                }
                catch
                {

                }
            }
        }

        private void checkBoxError_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxError.Checked)
            {
                try
                {
                    wavPlayer.SoundLocation = @textBoxErrorFile.Text;
                    wavPlayer.Play();
                }
                catch
                {

                }
            }
        }

        private void openFileDialogWAV_FileOk(object sender, CancelEventArgs e)
        {
            destSoundTextBox.Text = openFileDialogWAV.FileName;
        }
    }
}