using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;
using USB = PICkit2V2.USB;

namespace PICkit2V2
{
    public partial class DialogTroubleshoot : Form
    {
        public DialogTroubleshoot()
        {
            InitializeComponent();
            this.Text = Pk2.ToolName + " Troubleshooting";
            this.labelIntroTitle.Text = Pk2.ToolName + " Troubleshooting";
            labelIntroText1.Text = "This wizard can be used to help insure that your\n" + Pk2.ToolName +
                                    " is operating properly, and that all necessary ICSP\n" +
                                    "programming signals are properly connected to your\n" +
                                    "PIC® microcontroller device.\n\n" +
                                    "To verify that the " + Pk2.ToolName + " is working properly, the " + Pk2.ToolName + " \n" +
                                    "programmer should be disconnected from any target device\n" +
                                    "and circuitry.  In the following steps check the outputs of\n" +
                                    "ICSP connector with a voltmeter and oscilloscope.\n\n" +
                                    "To verify connectivity from the " + Pk2.ToolName + " to your PIC®\n" +
                                    "microcontroller device, connect the " + Pk2.ToolName + " ICSP connector\n" +
                                    "to the target.Verify all signals reach all appropriate device\n" +
                                    "pins with a voltmeter and oscilloscope.";
            Pk2.VddOff();
            byte[] pinscript = new byte[2];
            pinscript[0] = KONST._SET_ICSP_PINS;
            pinscript[1] = 0x03;        // set both pins to inputs.
            Pk2.SendScript(pinscript);
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
                testVDD();
            }
            else if (panelStep1VDDTest.Visible)
            {
                Pk2.VddOff();
                panelStep1VDDTest.Visible = false;
                panelCautionVDD.Visible = true;            
            }
            else if (panelStep1VDDExt.Visible)
            {
                Pk2.VddOff();
                panelStep1VDDExt.Visible = false;
                panelStep2VPP.Visible = true;
                testVPP_Enter();
            }            
            else if (panelCautionVDD.Visible)
            {
                panelCautionVDD.Visible = false;
                panelStep2VPP.Visible = true;
                testVPP_Enter();
            }
            else if (panelStep2VPP.Visible)
            {
                panelStep2VPP.Visible = false;
                panelPGCPGD.Visible = true;
                buttonNext.Enabled = false;
                testPGCPGDEnter();
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (panelStep1VDDExt.Visible || panelStep1VDDTest.Visible)
            {
                // Shut off VDD
                Pk2.VddOff();
                panelIntro.Visible = true;
                buttonBack.Enabled = false;
                panelStep1VDDTest.Visible = false;
                panelStep1VDDExt.Visible = false;
            }
            else if (panelCautionVDD.Visible || panelStep2VPP.Visible)
            {
                panelCautionVDD.Visible = false;
                panelStep2VPP.Visible = false;
                testVDD();
            }
            else if (panelPGCPGD.Visible)
            {
                panelPGCPGD.Visible = false;
                panelStep2VPP.Visible = true;
                buttonNext.Enabled = true;
                testVPP_Enter();
            }
        }
        
        // ######################  VDD TEST  ##########################
        private void testVDD()
        {
            float vdd = 0;
            float vpp = 0;

            // ensure VPP is off.
            byte[] vppscript = new byte[4];
            vppscript[0] = KONST._VPP_OFF;
            vppscript[1] = KONST._VPP_PWM_OFF;
            vppscript[2] = KONST._VDD_OFF;
            vppscript[3] = KONST._VDD_GND_ON;
            Pk2.SendScript(vppscript);
            
            Thread.Sleep(250); // sleep a bit to let VDD bleed down.
            
            // Set numericUpDown1 here and not in 'else' below. Otherwise Vpp test will use incorrect value
            // if Vdd test skipped because of powered target.
            /*numericUpDown1.Maximum = (decimal)Pk2.DevFile.PartsList[Pk2.ActivePart].VddMax;
            numericUpDown1.Minimum = (decimal)Pk2.DevFile.PartsList[Pk2.ActivePart].VddMin;
            if ((float)numericUpDown1.Maximum > 4.5F)
            {
                numericUpDown1.Value = (decimal)4.5;
            }
            else
            {
                numericUpDown1.Value = numericUpDown1.Maximum;
            }*/

            if (Pk2.CheckTargetPower(ref vdd, ref vpp) == KONST.PICkit2PWR.selfpowered)
            {
                panelStep1VDDExt.Visible = true;
                labelVoltageOnVDD.Text = "An external voltage was detected\non the VDD pin at " 
                        + string.Format("{0:0.0} Volts.", vdd);
                numericUpDown1.Value = (decimal)vdd;
            }
            else
            {
                panelStep1VDDExt.Visible = false;
                panelStep1VDDTest.Visible = true;
                labelGood.Visible = false;
                labelVDDShort.Visible = false;
                labelVDDLow.Visible = false;
                labelReadVDD.Text = "";
                numericUpDown1.Maximum = (decimal) Pk2.DevFile.PartsList[Pk2.ActivePart].VddMax;
                numericUpDown1.Minimum = (decimal)Pk2.DevFile.PartsList[Pk2.ActivePart].VddMin;
                if ((float)numericUpDown1.Maximum > 4.5F)
                {
                    numericUpDown1.Value = (decimal)4.5;
                }
                else
                {
                    numericUpDown1.Value = numericUpDown1.Maximum;
                }
            }
        }

        private void buttonStep1Recheck_Click(object sender, EventArgs e)
        {
            testVDD();
        }

        private void buttonVDDOn_Click(object sender, EventArgs e)
        {
            float vdd = 0;
            float vpp = 0;        
        
            labelGood.Visible = false;
            labelVDDShort.Visible = false;
            labelVDDLow.Visible = false;
            labelReadVDD.Text = "";        
            
            // set VDD with a low threshold
            float voltage = (float) numericUpDown1.Value;
            if (Pk2.SetVDDVoltage(voltage, 0.45F, false))
            {
            
                // turn on VDD
                Pk2.ForcePICkitPowered();
                if (Pk2.VddOn())
                {
                    // check status first for shorts
                    if (Pk2.PowerStatus() != KONST.PICkit2PWR.vdd_on)
                    { //short!
                        labelVDDShort.Visible = true;
                        labelReadVDD.Text = "Short!";
                    }
                    else
                    {// status OK, read VDD voltage
                        if (Pk2.ReadPICkitVoltages(ref vdd, ref vpp))
                        {
                            labelReadVDD.Text = string.Format("{0:0.0} V", vdd);
                            float expectedVDD = (float)numericUpDown1.Value;
                            if (expectedVDD > 4.6F)
                            {
                                expectedVDD = 4.6F;  // There is a typical drop for VDD > 4.6V
                                                     // don't count that
                            }
                            if ((expectedVDD - vdd) > 0.2F)
                            {
                                labelVDDLow.Visible = true;
                            }
                            else
                            {
                                labelGood.Visible = true;
                                labelGood.Text = "Test Passed:\r\n" + Pk2.ToolName + " detected an expected voltage\r\non the VDD pin.  (NOTE: slow" +
                                                " rise\r\ntimes can still cause VDD Errors.)\r\nClick \"Next >\" to test VPP.";
                            }                                
                        }
                        
                    }
                }
            
            }

        }

        // ######################  VPP TEST  ##########################     
        
        private void testVPP_Enter()
        {
            Pk2.VddOff();
            byte[] pinscript = new byte[2];
            pinscript[0] = KONST._SET_ICSP_PINS;
            pinscript[1] = 0x03;        // set both pins to inputs.
            Pk2.SendScript(pinscript);
            
            timerPGxToggle.Enabled = false;
            
            buttonCancel.Text = "Cancel";
            
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].Vpp < 1)
            {
                labelStep2FamilyVPP.Text = "1) VPP for this family: "
                        + string.Format("{0:0.0}V (=VDD)", numericUpDown1.Value);
            }
            else
            {
            labelStep2FamilyVPP.Text = "1) VPP for this family: "
                    + string.Format("{0:0.0} Volts.", Pk2.DevFile.Families[Pk2.GetActiveFamily()].Vpp);
            }
            labelReadVPP.Text = "";
            labelVPPLow.Visible = false;
            labelVPPMCLR.Visible = false;
            labelVPPMCLROff.Visible = false;
            labelVPPPass.Visible = false;
            labelVPPShort.Visible = false;
            labelVPPVDDShort.Visible = false;
        }       

        private void buttonTestVPP_Click(object sender, EventArgs e)
        {
            float vdd = 0;
            float vpp = 0;

            labelVPPLow.Visible = false;
            labelVPPMCLR.Visible = false;
            labelVPPMCLROff.Visible = false;
            labelVPPPass.Visible = false;
            labelVPPShort.Visible = false;
            labelVPPVDDShort.Visible = false;
            labelReadVPP.Text = "";

            Thread.Sleep(250); // sleep a bit to let VDD bleed down.
            
            // check for a powered target first
            if (Pk2.CheckTargetPower(ref vdd, ref vpp) == KONST.PICkit2PWR.selfpowered)
            {
                Pk2.VddOff();
            } 
            else
            {
                Pk2.SetVDDVoltage((float)numericUpDown1.Value, 0.85F, false);
                Pk2.VddOn();
            }
            // Set VPP voltage
            float expectedVPP;
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].Vpp > 1)
            {
                expectedVPP = Pk2.DevFile.Families[Pk2.GetActiveFamily()].Vpp;
            }
            else
            {
                expectedVPP = (float)numericUpDown1.Value;
            }
            Pk2.SetVppVoltage(expectedVPP, 0.50F);
            byte[] vppscript = new byte[8];
            vppscript[0] = KONST._VPP_OFF;
            vppscript[1] = KONST._VPP_PWM_ON;
            vppscript[2] = KONST._DELAY_LONG;
            vppscript[3] = 30;
            vppscript[4] = KONST._MCLR_GND_OFF;
            vppscript[5] = KONST._VPP_ON;
            vppscript[6] = KONST._DELAY_LONG;
            vppscript[7] = 20;            
            Pk2.SendScript(vppscript);

            // check status first for shorts
            KONST.PICkit2PWR status = Pk2.PowerStatus();
            if ((status == KONST.PICkit2PWR.vdderror) || (status == Constants.PICkit2PWR.vddvpperrors))
            { //VDD short!
                labelVPPVDDShort.Visible = true;
            }      
            else if (status == KONST.PICkit2PWR.vpperror)
            {//VPP short
                labelVPPShort.Visible = true;
                labelReadVPP.Text = "Short!";
            }     
            else if (status != Constants.PICkit2PWR.no_response)
            {   // status OK, read VPP voltage
                if (Pk2.ReadPICkitVoltages(ref vdd, ref vpp))
                {
                    labelReadVPP.Text = string.Format("{0:0.0} V", vpp);
                    if ((expectedVPP - vpp) > 0.3F)
                    {
                        labelVPPLow.Visible = true;
                    }
                    else
                    {
                        labelVPPPass.Visible = true;
                    }
                }                
            
            }
            
        }

        private void buttonMCLR_Click(object sender, EventArgs e)
        {
            labelVPPLow.Visible = false;
            labelVPPMCLR.Visible = true;
            labelVPPMCLROff.Visible = false;
            labelVPPPass.Visible = false;
            labelVPPShort.Visible = false;
            labelVPPVDDShort.Visible = false;
            labelReadVPP.Text = "/MCLR On";

            // Ensure VPP is off and assert /MCLR
            byte[] vppscript = new byte[3];
            vppscript[0] = KONST._VPP_OFF;
            vppscript[1] = KONST._VPP_PWM_OFF;
            vppscript[2] = KONST._MCLR_GND_ON;
            Pk2.SendScript(vppscript);
        }

        private void buttonMCLROff_Click(object sender, EventArgs e)
        {
            labelVPPLow.Visible = false;
            labelVPPMCLR.Visible = false;
            labelVPPMCLROff.Visible = true;
            labelVPPPass.Visible = false;
            labelVPPShort.Visible = false;
            labelVPPVDDShort.Visible = false;
            labelReadVPP.Text = "/MCLR Off";

            // Ensure VPP is off and de-assert /MCLR
            byte[] vppscript = new byte[3];
            vppscript[0] = KONST._VPP_OFF;
            vppscript[1] = KONST._VPP_PWM_OFF;
            vppscript[2] = KONST._MCLR_GND_OFF;
            Pk2.SendScript(vppscript);
        }        

        // ###################### PGx TEST  ########################## 
        private void testPGCPGDEnter()
        {
            float vdd = 0;
            float vpp = 0;
        
            // set VPP off
            byte[] vppscript = new byte[3];
            vppscript[0] = KONST._VPP_OFF;
            vppscript[1] = KONST._VPP_PWM_OFF;
            vppscript[2] = KONST._MCLR_GND_ON;
            Pk2.SendScript(vppscript);
            
            Pk2.VddOff();

            buttonCancel.Text = "Finished";

            Thread.Sleep(200); // sleep a bit to let VDD bleed down.

            // check for a powered target first
            if (Pk2.CheckTargetPower(ref vdd, ref vpp) == KONST.PICkit2PWR.selfpowered)
            {
                Pk2.VddOff();
            }
            else
            {
                Pk2.SetVDDVoltage((float)numericUpDown1.Value, 0.85F, false);
                Pk2.VddOn();
                Thread.Sleep(50);
            }

            // check status next for shorts
            KONST.PICkit2PWR status = Pk2.PowerStatus();
            if ((status == KONST.PICkit2PWR.vdderror) || (status == Constants.PICkit2PWR.vddvpperrors))
            { //VDD short!
                radioButtonPGCHigh.Enabled = false;
                radioButtonPGCLow.Enabled = false;
                radioButtonPGDHigh.Enabled = false;
                radioButtonPGDLow.Enabled = false;
                radioButtonPGCToggle.Enabled = false;
                radioButtonPGDToggle.Enabled = false;
                labelPGxOScope.Visible = false;
                labelPGxVDDShort.Visible = true;
            }
            else if (status == KONST.PICkit2PWR.vpperror)
            {//VPP short
                radioButtonPGCHigh.Enabled = false;
                radioButtonPGCLow.Enabled = false;
                radioButtonPGDHigh.Enabled = false;
                radioButtonPGDLow.Enabled = false;
                radioButtonPGCToggle.Enabled = false;
                radioButtonPGDToggle.Enabled = false;
                labelPGxOScope.Visible = false;
                labelPGxVDDShort.Visible = true;                
            }
            else if (status != Constants.PICkit2PWR.no_response)
            {   // status OK, Set PGC/PGC pins outputs low.

                radioButtonPGCHigh.Enabled = true;
                radioButtonPGCLow.Enabled = true;
                radioButtonPGDHigh.Enabled = true;
                radioButtonPGDLow.Enabled = true;
                radioButtonPGCToggle.Enabled = true;
                radioButtonPGDToggle.Enabled = true;
                labelPGxOScope.Visible = true;
                labelPGxVDDShort.Visible = false;
            
                vppscript[0] = KONST._SET_ICSP_PINS;
                vppscript[1] = 0x00;
                vppscript[2] = KONST._BUSY_LED_OFF; // "NOP"
                Pk2.SendScript(vppscript);
                
                radioButtonPGDToggle.Checked = false;
                radioButtonPGCToggle.Checked = false;
                radioButtonPGCHigh.Checked = false;
                radioButtonPGCLow.Checked = true;
                radioButtonPGDHigh.Checked = false;
                radioButtonPGDLow.Checked = true;
            }            
            
            
        }

        private void radioButtonPGCHigh_CheckedChanged(object sender, EventArgs e)
        {
            byte[] pgcscript = new byte[2];
            
            if (radioButtonPGDToggle.Checked || radioButtonPGCToggle.Checked)
            {
                return; // don't do anything
            }
            else
            { // ensure timer is off and set pin states!
                timerPGxToggle.Enabled = false;

                pgcscript[0] = KONST._SET_ICSP_PINS;
            
                if (radioButtonPGCHigh.Checked && radioButtonPGDHigh.Checked)
                {
                    pgcscript[1] = 0x0C;  
                }
                else if (radioButtonPGCHigh.Checked)
                {
                    pgcscript[1] = 0x04;
                }
                else if (radioButtonPGDHigh.Checked)
                {
                    pgcscript[1] = 0x08;
                }
                else
                {
                    pgcscript[1] = 0x00;
                }
                Pk2.SendScript(pgcscript);
            }
        }

        private void radioButtonPGDToggle_Click(object sender, EventArgs e)
        {
            PGxToggle();
        }

        private void timerPGxToggle_Tick(object sender, EventArgs e)
        {
            PGxToggle();
        }

        private void PGxToggle()
        {
            timerPGxToggle.Enabled = false;
        
            byte pgxvaluehigh = 0x0;
            byte pgxvaluelow = 0x0;
            if (radioButtonPGDToggle.Checked)
            {
                pgxvaluehigh |= 0x8;
            }
            if (radioButtonPGCToggle.Checked)
            {
                pgxvaluehigh |= 0x4;
            }
            if (radioButtonPGCHigh.Checked)
            {
                pgxvaluehigh |= 0x4;
                pgxvaluelow |= 0x4;
            }
            if (radioButtonPGDHigh.Checked)
            {
                pgxvaluehigh |= 0x8;
                pgxvaluelow |= 0x8;
            }

            // 30kHz square wave - 500ms @ 30k^-1 * 256 * 59
            byte[] pgdscript = new byte[17];
            pgdscript[0] = KONST._CONST_WRITE_DL;
            pgdscript[1] = 59;
            pgdscript[2] = KONST._CONST_WRITE_DL;
            pgdscript[3] = 0;
            pgdscript[4] = KONST._SET_ICSP_PINS;
            pgdscript[5] = pgxvaluehigh;
            pgdscript[6] = KONST._BUSY_LED_ON;         // a NOP to stretch the high pulse time.
            pgdscript[7] = KONST._BUSY_LED_ON;         // a NOP
            pgdscript[8] = KONST._SET_ICSP_PINS;        
            pgdscript[9] = pgxvaluelow;
            pgdscript[10] = KONST._BUSY_LED_ON;         // a NOP
            pgdscript[11] = KONST._LOOP;
            pgdscript[12] = 7;
            pgdscript[13] = 0;  //256
            pgdscript[14] = KONST._LOOPBUFFER;
            pgdscript[15] = 10;
            pgdscript[16] = KONST._BUSY_LED_OFF;
            Pk2.SendScript(pgdscript);

            timerPGxToggle.Enabled = true;
        }             

        private void trblshtingFormClosing(object sender, FormClosingEventArgs e)
        {
            // clean up - shut down everything
            timerPGxToggle.Enabled = false;
            
            byte[] closescript = new byte[5];
            closescript[0] = KONST._VPP_OFF;
            closescript[1] = KONST._MCLR_GND_OFF;
            closescript[2] = KONST._VPP_PWM_OFF;
            closescript[3] = KONST._SET_ICSP_PINS;
            closescript[4] = 0x03;                  // inputs
            Pk2.SendScript(closescript);
            
            Pk2.VddOff();
        }


        
    }
}