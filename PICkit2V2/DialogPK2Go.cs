using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;

namespace PICkit2V2
{
    public partial class DialogPK2Go : Form
    {
        public float VDDVolts = 0;
        public string dataSource = "--";
        public bool codeProtect = false;
        public bool dataProtect = false;
        public bool verifyDevice = false;
        public bool vppFirst = false;
        public bool writeProgMem = true;
        public bool writeEEPROM = true;
        public bool fastProgramming = true;
        public bool holdMCLR = false;
        public byte icspSpeedSlow = 4;

        private byte ptgMemory = 0; // 128K default
        
        private int blinkCount = 0;
    
        public DialogPK2Go()
        {
            InitializeComponent();
            if (Pk2.isPK3)
            {
                this.label2.Text = "To change PICkit 3 VDD voltage, click\r\n    CANCEL and adjust the VDD box.\r\n";
                this.label16.Text = "Welcome to the PICkit 3";
                this.radioButtonPK2Power.Text = "Power target from PICkit 3 at 0.0 Volts";
                this.label7.Text = "Click the DOWNLOAD button below to\r\nset up PICkit 3 for Programmer-To-Go\r\noperation.\r\n";
                this.label12.Text = "Download to PICkit 3";
                this.label19.Text = "Remove the PICkit 3 from USB now.";
                this.label17.Text = "The PICkit 3 unit will indicate it\'s in Programmer-To-Go\r\nmode and ready to program by blinking the \"Active\" \r\nLED twice in succession:";
                this.label22.Text = "Basic Operation Steps:\r\n   1) Connect USB power to the PICkit 3(See Help)\r\n   2) Check for Active LED blinking(PICkit 3 ready)\r\n   3) Connect PICkit 3 ICSP connector to the target.\r\n   4) Press the PICkit 3 button to begin programming\r\nDuring programming the \"Status\" LED will remain lit.\r\nOnce programming has completed, either\r\n     -The Active LED blinks, indicating success\r\n     -The Status LED blinks, indicating an error.\r\n       Press the PICkit 3 button to clear the error.";
                this.label23.Text = "Status";

            }
            else
            {
                this.label2.Text = "To change PICkit 2 VDD voltage, click\r\n    CANCEL and adjust the VDD box.\r\n";
                this.label16.Text = "Welcome to the PICkit 2";
                this.radioButtonPK2Power.Text = "Power target from PICkit 2 at 0.0 Volts";
                this.label7.Text = "Click the DOWNLOAD button below to\r\nset up PICkit 2 for Programmer-To-Go\r\noperation.\r\n";
                this.label12.Text = "Download to PICkit 2";
                this.label19.Text = "Remove the PICkit 2 from USB now.";
                this.label17.Text = "The PICkit 2 unit will indicate it\'s in Programmer-To-Go\r\nmode and ready to program by blinking the \"Target\" \r\nLED twice in succession:";
                this.label22.Text = "Basic Operation Steps:\r\n   1) Connect USB power to the PICkit 2(See Help)\r\n   2) Check for Target LED blinking(PICkit 2 ready)\r\n   3) Connect PICkit 2 ICSP connector to the target.\r\n   4) Press the PICkit 2 button to begin programming\r\nDuring programming the \"Busy\" LED will remain lit.\r\nOnce programming has completed, either\r\n     -The Target LED blinks, indicating success\r\n     -The Busy LED blinks, indicating an error.\r\n       Press the PICkit 2 button to clear the error.";
                this.label23.Text = "Busy";
            }
        }
        
        public void SetPTGMemory(byte value)
        {
            ptgMemory = value;
            if (Pk2.isPK3)
                ptgMemory = 5;
            if ((ptgMemory > 0) && (ptgMemory <= 5))
                label256K.Visible = true;
            //===== Display what will be used for PTG =====
            if (ptgMemory == 1) label256K.Text = "256K PICkit 2 upgrade support enabled.\r\n";
            else if (ptgMemory == 2) label256K.Text = "512K SPI memory support enabled.\r\n";
            else if (ptgMemory == 3) label256K.Text = "1M SPI memory support enabled.\r\n";
            else if (ptgMemory == 4) label256K.Text = "2M SPI memory support enabled.\r\n";
            else if (ptgMemory == 5) label256K.Text = "4M SPI memory support enabled.\r\n";

            if (Pk2.EEPROMNotFound())        // PTG EEPROM missing
            {
                if (Pk2.isPK3)
                    label256K.Text = "PICkit3 code FLASH not found!\r\n";
                else
                    label256K.Text = "PICkit2 EEPROM not found!\r\n";
                buttonNext.Enabled = false;
            }

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (panelIntro.Visible)
            {
                panelIntro.Visible = false;
                buttonBack.Enabled = true;
                fillSettings(true);
            }
            else if (panelSettings.Visible)
            {
                if (checkEraseVoltage())
                {
                    panelSettings.Visible = false;
                    buttonNext.Text = "Download";
                    fillDownload();
                }
            }
            else if (panelDownload.Visible)
            {
                downloadGO();
            }       
            else if (panelDownloadDone.Visible)
            {
                buttonNext.Enabled = false;
                panelDownloadDone.Visible = false;
                panelErrors.Visible = true;
                timerBlink.Interval = 84;
            }     
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (panelSettings.Visible)
            {
                panelSettings.Visible = false;
                panelIntro.Visible = true;
                buttonBack.Enabled = false;
                buttonNext.Enabled = true;
            }
            else if (panelDownload.Visible)
            {
                panelDownload.Visible = false;
                buttonNext.Text = "Next >";
                fillSettings(false);
            }
        }
        
        private bool checkEraseVoltage()
        {
            if (radioButtonSelfPower.Checked)
                return true;
            if (VDDVolts < Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase)
            {
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].DebugRowEraseScript == 0)
                {
                    DialogResult goAhead = MessageBox.Show(
                        "The selected "+ Pk2.ToolName + " VDD voltage is below\nthe minimum required to Bulk Erase this part.\n\nContinue anyway?",
                        labelPartNumber.Text + " VDD Error", MessageBoxButtons.OKCancel);
                        
                    if (goAhead == DialogResult.OK)
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }
        
        private void fillSettings(bool changePower)
        {
            // Buffer settings
            // Device
            labelPartNumber.Text = Pk2.DevFile.PartsList[Pk2.ActivePart].PartName;
            if (Pk2.DevFile.PartsList[Pk2.ActivePart].OSSCALSave)
            {
                labelOSCCAL_BandGap.Visible = true;
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].BandGapMask > 0)
                {
                    labelOSCCAL_BandGap.Text = "OSCCAL && BandGap will be preserved.";
                }

            }
            // source
            if (dataSource == "Edited.")
                labelDataSource.Text = "Edited Buffer.";
            else
                labelDataSource.Text = dataSource;
            // code protects
            if (!writeProgMem)
            { // write EE only
                labelCodeProtect.Text = "N/A";
                labelDataProtect.Text = "N/A";
            }
            else
            {
                if (codeProtect)
                    labelCodeProtect.Text = "ON";
                else
                    labelCodeProtect.Text = "OFF";
                if (dataProtect)
                    labelDataProtect.Text = "ON";
                else
                {
                    if (Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem > 0)
                        labelDataProtect.Text = "OFF";                
                    else
                        labelDataProtect.Text = "N/A";
                }
            }
            // mem regions
            if (!writeProgMem)
            {
                labelMemRegions.Text = "Write EEPROM data only.";
            }
            else if (!writeEEPROM && (Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem > 0))
            {
                labelMemRegions.Text = "Preserve EEPROM on write.";
            }
            else
            {
                labelMemRegions.Text = "Write entire device.";
            } 
            if (verifyDevice)
                labelVerify.Text = "Yes";
            else
                labelVerify.Text = "No - device will NOT be verified";

            // Power Settings
            if (changePower)
            {
                radioButtonPK2Power.Text = string.Format("Power target from " + Pk2.ToolName + " at {0:0.0} Volts.", VDDVolts);
                if (vppFirst)
                {
                    radioButtonSelfPower.Enabled = false;
                    radioButtonSelfPower.Text = "Use VPP First - must power from " + Pk2.ToolName;
                    checkBoxRowErase.Enabled = false;
                    radioButtonPK2Power.Checked = true;
                    pickit2PowerRowErase();
                }
                else
                {
                    radioButtonSelfPower.Checked = true;
                    if (Pk2.DevFile.PartsList[Pk2.ActivePart].DebugRowEraseScript > 0)
                    {
                        checkBoxRowErase.Text = string.Format("VDD < {0:0.0}V: Use low voltage row erase", 
                                    Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase);
                        checkBoxRowErase.Enabled = true;
                    }
                    else
                    {
                        checkBoxRowErase.Visible = false;
                        checkBoxRowErase.Enabled = false;
                        labelVDDMin.Text = string.Format("VDD must be >= {0:0.0} Volts.", Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase);
                        labelVDDMin.Visible = true;
                    }
                }
            }
            panelSettings.Visible = true;
        }
        
        private bool pickit2PowerRowErase()
        {
            if (VDDVolts < Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase)
            {
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].DebugRowEraseScript > 0)
                {
                    labelRowErase.Text = "Row Erase used: Will NOT program Code Protected parts!";
                    labelRowErase.Visible = true;
                }
                else
                {
                    MessageBox.Show(string.Format(Pk2.ToolName + " cannot program this device\nat the selected VDD voltage.\n\n{0:0.0}V is below the minimum for erase, {0:0.0}V", 
                        VDDVolts, Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase), "Programmer-To-Go");
                    return false;
                }
            }
            else
            {
                labelRowErase.Visible = false;
            }
            return true;
        }
        
        private void fillDownload()
        {
            labelPNsmmry.Text = labelPartNumber.Text;
            labelSourceSmmry.Text = labelDataSource.Text;
            
            if (radioButtonSelfPower.Checked)
            {
                if (checkBoxRowErase.Enabled && checkBoxRowErase.Checked)
                {
                    labelTargetPowerSmmry.Text = "Target is Powered (Use Low Voltage Row Erase)";
                }
                else
                {
                    labelTargetPowerSmmry.Text = string.Format("Target is Powered (Min VDD = {0:0.0} Volts)", Pk2.DevFile.PartsList[Pk2.ActivePart].VddErase);
                }
            }
            else
            {
                labelTargetPowerSmmry.Text = string.Format("Power target from " + Pk2.ToolName + " at {0:0.0} Volts", VDDVolts);
            }
            
            labelMemRegionsSmmry.Text = labelMemRegions.Text;
            
            if (writeProgMem)
            {
                if (codeProtect)
                    labelMemRegionsSmmry.Text += " -CP";
                if (dataProtect)
                    labelMemRegionsSmmry.Text += " -DP";
            }
            
            if (vppFirst)
                labelVPP1stSmmry.Text = "Use VPP 1st Program Entry";
            else
                labelVPP1stSmmry.Text = "";
                
            if (verifyDevice)
                labelVerifySmmry.Text = "Device will be verified";
            else
                labelVerifySmmry.Text = "Device will NOT be verified";
                
            if (fastProgramming)
                labelFastProgSmmry.Text = "Fast Programming is ON";
            else
                labelFastProgSmmry.Text = "Fast Programming is OFF";
                
            if (holdMCLR)
                labelMCLRHoldSmmry.Text = "MCLR kept asserted during && after programming";
            else
                labelMCLRHoldSmmry.Text = "MCLR released after programming";
            
            panelDownload.Visible = true;
        }
        
        public DelegateWrite PICkit2WriteGo;
        
        private void downloadGO()
        {
            panelDownload.Visible = false;
            panelDownloading.Visible = true;
            buttonHelp.Enabled = false;
            buttonBack.Enabled = false;
            buttonNext.Enabled = false;
            buttonCancel.Enabled = false;
            buttonCancel.Text = "Exit";
            this.Update();
        
            if (radioButtonSelfPower.Checked)
            {
                Pk2.ForceTargetPowered();
            }
            else
            {
                Pk2.ForcePICkitPowered();
            }
            if (ptgMemory <= 5)
                Pk2.EnterLearnMode(ptgMemory); // set memory size to use
            else
                Pk2.EnterLearnMode(0); // default to 128K on illegal value

            if (fastProgramming)
                Pk2.SetProgrammingSpeed(0);
            else
                Pk2.SetProgrammingSpeed(icspSpeedSlow);
            
            PICkit2WriteGo(true);
            
            Pk2.ExitLearnMode();
            
            if (ptgMemory <= 5)
                Pk2.EnablePK2GoMode(ptgMemory); // set memory size to use
            else
                Pk2.EnablePK2GoMode(0); // default to 128K on illegal value.

            Pk2.DisconnectPICkit2Unit();
            
            panelDownloading.Visible = false;
            panelDownloadDone.Visible = true;
            buttonHelp.Enabled = true;
            buttonNext.Enabled = true;
            buttonNext.Text = "Next >";
            buttonCancel.Enabled = true;      
            timerBlink.Enabled = true;      
        }

        private void radioButtonPK2Power_Click(object sender, EventArgs e)
        {
            radiobuttonPower();
        }

        private void radioButtonSelfPower_Click(object sender, EventArgs e)
        {
            radiobuttonPower();
        }        
        
        private void radiobuttonPower()
        {
                    if (radioButtonPK2Power.Checked)
            {
                checkBoxRowErase.Enabled = false;
                if (!pickit2PowerRowErase())
                {
                    radioButtonPK2Power.Checked = false;
                    radioButtonSelfPower.Checked = true;
                }
            }
            else
            {
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].DebugRowEraseScript > 0)
                {
                    checkBoxRowErase.Enabled = true;
                }
                else
                {
                    checkBoxRowErase.Enabled = false;
                }
            
                if (checkBoxRowErase.Enabled && checkBoxRowErase.Checked)
                {
                    labelRowErase.Text = "Row Erase used: Will NOT program Code Protected parts!";
                    labelRowErase.Visible = true;
                }
                else
                {
                    labelRowErase.Visible = false;
                }
            }
        }

        private void checkBoxRowErase_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRowErase.Enabled && checkBoxRowErase.Checked)
            {
                labelRowErase.Text = "Row Erase used: Will NOT program Code Protected parts!";
                labelRowErase.Visible = true;
            }
            else
            {
                labelRowErase.Visible = false;
            }
        }

        private void timerBlink_Tick(object sender, EventArgs e)
        {
            if (panelDownloadDone.Visible)
            {
                blinkCount++;
                if (blinkCount > 5)
                    blinkCount = 0;
                if (blinkCount < 4)
                {
                    if ((blinkCount & 0x1) == 0)
                    {
                        if (Pk2.isPK3)
                        {
                            pictureBoxTarget.BackColor = Color.Blue;
                            label18.Text = "Active";
                        }
                        else
                        {
                            pictureBoxTarget.BackColor = Color.Yellow;
                            label18.Text = "Target";
                        }
                    }
                    else
                        pictureBoxTarget.BackColor = System.Drawing.SystemColors.ControlText;
                }
            }
            else
            { // error panel
                if (radioButtonVErr.Checked)
                {
                    blinkCount++;
                    if ((blinkCount & 0x1) == 0)
                        pictureBoxBusy.BackColor = Color.Red;
                    else
                        pictureBoxBusy.BackColor = System.Drawing.SystemColors.ControlText;
                }
                else
                {
                    int blink = 4;
                    if (radioButton3Blinks.Checked)
                        blink = 6;
                    else if (radioButton4Blinks.Checked)
                        blink = 8;
                    if (blinkCount++ <= blink)
                    {
                        if ((blinkCount & 0x1) == 0)
                            pictureBoxBusy.BackColor = Color.Red;
                        else
                            pictureBoxBusy.BackColor = System.Drawing.SystemColors.ControlText;
                    }
                    else
                        blinkCount = 0;
                }   
            }

        }

        private void DialogPK2Go_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pk2.ExitLearnMode(); // just in case.
        }

        private void radioButtonVErr_Click(object sender, EventArgs e)
        {
            if (radioButtonVErr.Checked)
                timerBlink.Interval = 84;
            else
                timerBlink.Interval = 200;
        }

        public DelegateOpenProgToGoGuide OpenProgToGoGuide;

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            OpenProgToGoGuide();
        }

    }
}