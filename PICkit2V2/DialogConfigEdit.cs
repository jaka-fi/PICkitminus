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
    public partial class DialogConfigEdit : Form
    {
        public float ScalefactW = 1F;   // scaling factors for dealing with non-standard DPI
        public float ScalefactH = 1F;   // these must be set before SetDisplayMask()is called
    
        private int displayMask; // 0 = unimplimented bits to 0, 1 = as '1', 2 = as read
        private const int K_MAXCONFIGS = 9;

        private byte editConfigWords = K_MAXCONFIGS;   // We adjust max K_MAXCONFIGS words

        private struct config {
            public Panel configPanel;
            public Label name;
            public Label addr;
            public Label value;
            public TextBox[] bits;
            }

        private config[] configWords = new config[K_MAXCONFIGS];
        private uint[] configSaves = new uint[K_MAXCONFIGS];
        private bool saveChanges = false; // not unless they click SAVE
    
        public DialogConfigEdit()
        {
            InitializeComponent();
            // JAKA
            editConfigWords = Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords;
            if (editConfigWords > 9)
            {
                editConfigWords = 9;
            }
            // END JAKA
            
            // first, save off existing config word values
            for (int cw = 0; cw < editConfigWords; cw++)
            {
                configSaves[cw] = Pk2.DeviceBuffers.ConfigWords[cw];
            }
            
            // Set up dialog arrays
            configWords[0].configPanel = panel1;
            configWords[0].name = labelName1;
            configWords[0].addr = labelAdr1;
            configWords[0].value = labelVal1;
            configWords[0].bits = new TextBox[16];
            configWords[0].bits[0] = textBox1_0;
            configWords[0].bits[1] = textBox1_1;
            configWords[0].bits[2] = textBox1_2;
            configWords[0].bits[3] = textBox1_3;
            configWords[0].bits[4] = textBox1_4;
            configWords[0].bits[5] = textBox1_5;
            configWords[0].bits[6] = textBox1_6;
            configWords[0].bits[7] = textBox1_7;
            configWords[0].bits[8] = textBox1_8;
            configWords[0].bits[9] = textBox1_9;
            configWords[0].bits[10] = textBox1_10;
            configWords[0].bits[11] = textBox1_11;
            configWords[0].bits[12] = textBox1_12;
            configWords[0].bits[13] = textBox1_13;
            configWords[0].bits[14] = textBox1_14;
            configWords[0].bits[15] = textBox1_15;

            configWords[1].configPanel = panel2;
            configWords[1].name = labelName2;
            configWords[1].addr = labelAdr2;
            configWords[1].value = labelVal2;
            configWords[1].bits = new TextBox[16];
            configWords[1].bits[0] = textBox2_0;
            configWords[1].bits[1] = textBox2_1;
            configWords[1].bits[2] = textBox2_2;
            configWords[1].bits[3] = textBox2_3;
            configWords[1].bits[4] = textBox2_4;
            configWords[1].bits[5] = textBox2_5;
            configWords[1].bits[6] = textBox2_6;
            configWords[1].bits[7] = textBox2_7;
            configWords[1].bits[8] = textBox2_8;
            configWords[1].bits[9] = textBox2_9;
            configWords[1].bits[10] = textBox2_10;
            configWords[1].bits[11] = textBox2_11;
            configWords[1].bits[12] = textBox2_12;
            configWords[1].bits[13] = textBox2_13;
            configWords[1].bits[14] = textBox2_14;
            configWords[1].bits[15] = textBox2_15;

            configWords[2].configPanel = panel3;
            configWords[2].name = labelName3;
            configWords[2].addr = labelAdr3;
            configWords[2].value = labelVal3;
            configWords[2].bits = new TextBox[16];
            configWords[2].bits[0] = textBox3_0;
            configWords[2].bits[1] = textBox3_1;
            configWords[2].bits[2] = textBox3_2;
            configWords[2].bits[3] = textBox3_3;
            configWords[2].bits[4] = textBox3_4;
            configWords[2].bits[5] = textBox3_5;
            configWords[2].bits[6] = textBox3_6;
            configWords[2].bits[7] = textBox3_7;
            configWords[2].bits[8] = textBox3_8;
            configWords[2].bits[9] = textBox3_9;
            configWords[2].bits[10] = textBox3_10;
            configWords[2].bits[11] = textBox3_11;
            configWords[2].bits[12] = textBox3_12;
            configWords[2].bits[13] = textBox3_13;
            configWords[2].bits[14] = textBox3_14;
            configWords[2].bits[15] = textBox3_15;

            configWords[3].configPanel = panel4;
            configWords[3].name = labelName4;
            configWords[3].addr = labelAdr4;
            configWords[3].value = labelVal4;
            configWords[3].bits = new TextBox[16];
            configWords[3].bits[0] = textBox4_0;
            configWords[3].bits[1] = textBox4_1;
            configWords[3].bits[2] = textBox4_2;
            configWords[3].bits[3] = textBox4_3;
            configWords[3].bits[4] = textBox4_4;
            configWords[3].bits[5] = textBox4_5;
            configWords[3].bits[6] = textBox4_6;
            configWords[3].bits[7] = textBox4_7;
            configWords[3].bits[8] = textBox4_8;
            configWords[3].bits[9] = textBox4_9;
            configWords[3].bits[10] = textBox4_10;
            configWords[3].bits[11] = textBox4_11;
            configWords[3].bits[12] = textBox4_12;
            configWords[3].bits[13] = textBox4_13;
            configWords[3].bits[14] = textBox4_14;
            configWords[3].bits[15] = textBox4_15;

            configWords[4].configPanel = panel5;
            configWords[4].name = labelName5;
            configWords[4].addr = labelAdr5;
            configWords[4].value = labelVal5;
            configWords[4].bits = new TextBox[16];
            configWords[4].bits[0] = textBox5_0;
            configWords[4].bits[1] = textBox5_1;
            configWords[4].bits[2] = textBox5_2;
            configWords[4].bits[3] = textBox5_3;
            configWords[4].bits[4] = textBox5_4;
            configWords[4].bits[5] = textBox5_5;
            configWords[4].bits[6] = textBox5_6;
            configWords[4].bits[7] = textBox5_7;
            configWords[4].bits[8] = textBox5_8;
            configWords[4].bits[9] = textBox5_9;
            configWords[4].bits[10] = textBox5_10;
            configWords[4].bits[11] = textBox5_11;
            configWords[4].bits[12] = textBox5_12;
            configWords[4].bits[13] = textBox5_13;
            configWords[4].bits[14] = textBox5_14;
            configWords[4].bits[15] = textBox5_15;

            configWords[5].configPanel = panel6;
            configWords[5].name = labelName6;
            configWords[5].addr = labelAdr6;
            configWords[5].value = labelVal6;
            configWords[5].bits = new TextBox[16];
            configWords[5].bits[0] = textBox6_0;
            configWords[5].bits[1] = textBox6_1;
            configWords[5].bits[2] = textBox6_2;
            configWords[5].bits[3] = textBox6_3;
            configWords[5].bits[4] = textBox6_4;
            configWords[5].bits[5] = textBox6_5;
            configWords[5].bits[6] = textBox6_6;
            configWords[5].bits[7] = textBox6_7;
            configWords[5].bits[8] = textBox6_8;
            configWords[5].bits[9] = textBox6_9;
            configWords[5].bits[10] = textBox6_10;
            configWords[5].bits[11] = textBox6_11;
            configWords[5].bits[12] = textBox6_12;
            configWords[5].bits[13] = textBox6_13;
            configWords[5].bits[14] = textBox6_14;
            configWords[5].bits[15] = textBox6_15;

            configWords[6].configPanel = panel7;
            configWords[6].name = labelName7;
            configWords[6].addr = labelAdr7;
            configWords[6].value = labelVal7;
            configWords[6].bits = new TextBox[16];
            configWords[6].bits[0] = textBox7_0;
            configWords[6].bits[1] = textBox7_1;
            configWords[6].bits[2] = textBox7_2;
            configWords[6].bits[3] = textBox7_3;
            configWords[6].bits[4] = textBox7_4;
            configWords[6].bits[5] = textBox7_5;
            configWords[6].bits[6] = textBox7_6;
            configWords[6].bits[7] = textBox7_7;
            configWords[6].bits[8] = textBox7_8;
            configWords[6].bits[9] = textBox7_9;
            configWords[6].bits[10] = textBox7_10;
            configWords[6].bits[11] = textBox7_11;
            configWords[6].bits[12] = textBox7_12;
            configWords[6].bits[13] = textBox7_13;
            configWords[6].bits[14] = textBox7_14;
            configWords[6].bits[15] = textBox7_15;

            configWords[7].configPanel = panel8;
            configWords[7].name = labelName8;
            configWords[7].addr = labelAdr8;
            configWords[7].value = labelVal8;
            configWords[7].bits = new TextBox[16];
            configWords[7].bits[0] = textBox8_0;
            configWords[7].bits[1] = textBox8_1;
            configWords[7].bits[2] = textBox8_2;
            configWords[7].bits[3] = textBox8_3;
            configWords[7].bits[4] = textBox8_4;
            configWords[7].bits[5] = textBox8_5;
            configWords[7].bits[6] = textBox8_6;
            configWords[7].bits[7] = textBox8_7;
            configWords[7].bits[8] = textBox8_8;
            configWords[7].bits[9] = textBox8_9;
            configWords[7].bits[10] = textBox8_10;
            configWords[7].bits[11] = textBox8_11;
            configWords[7].bits[12] = textBox8_12;
            configWords[7].bits[13] = textBox8_13;
            configWords[7].bits[14] = textBox8_14;
            configWords[7].bits[15] = textBox8_15;

            configWords[8].configPanel = panel9;
            configWords[8].name = labelName9;
            configWords[8].addr = labelAdr9;
            configWords[8].value = labelVal9;
            configWords[8].bits = new TextBox[16];
            configWords[8].bits[0] = textBox9_0;
            configWords[8].bits[1] = textBox9_1;
            configWords[8].bits[2] = textBox9_2;
            configWords[8].bits[3] = textBox9_3;
            configWords[8].bits[4] = textBox9_4;
            configWords[8].bits[5] = textBox9_5;
            configWords[8].bits[6] = textBox9_6;
            configWords[8].bits[7] = textBox9_7;
            configWords[8].bits[8] = textBox9_8;
            configWords[8].bits[9] = textBox9_9;
            configWords[8].bits[10] = textBox9_10;
            configWords[8].bits[11] = textBox9_11;
            configWords[8].bits[12] = textBox9_12;
            configWords[8].bits[13] = textBox9_13;
            configWords[8].bits[14] = textBox9_14;
            configWords[8].bits[15] = textBox9_15;
            
            int tag = 0;
            for (int w = 0; w < K_MAXCONFIGS; w++)
            {// assign tags
                for (int b = 0; b < 16; b++)
                {
                    configWords[w].bits[b].Tag = tag++;
                }
            }
        }
        
        public void SetDisplayMask(int option)
        {
            displayMask = option;
            redraw();
        }
        
        private void redraw()
        {
            // Scale window to number of config words:
            int numUnusedConfigs = K_MAXCONFIGS - editConfigWords;
            numUnusedConfigs *= (int)(48 * ScalefactH);
            numUnusedConfigs -= (int)(24 * ScalefactH); // add a bit of length to the window for Asian OSs
            this.Size = new Size((int)(this.Size.Width),(int)(this.Size.Height - numUnusedConfigs));
        
            // defaults for Baseline, Midrange
            string[] cfgNames = new string[K_MAXCONFIGS];
            for (int i = 1; i <= cfgNames.Length; i++)
            {
                cfgNames[i-1] = string.Format("CONFIG{0:G}", i);
            }
        
            int configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr/2;
            int configIncrement = 1;
            
            if (editConfigWords == 1)
            { // baseline or midrange
                cfgNames[0] = "CONFIG";
            }
            
            // PIC18
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue == 0xFFFF)
            {
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].IgnoreBytes == 0x000C    // Config bytes instead of words
                    || (Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemPanelBufs & 0x02) == 0x02)
                {
                    cfgNames[0] = "CONFIG2+1";
                    cfgNames[1] = "CONFIG4+3";
                    cfgNames[2] = "CONFIG6+5";
                    cfgNames[3] = "CONFIG8+7";
                    cfgNames[4] = "CONFIG10+9";
                    cfgNames[5] = "CONFIG12+11";
                    cfgNames[6] = "CONFIG14+13";
                    cfgNames[7] = "CONFIG16+15";
                    cfgNames[8] = "CONFIG18+17";
                }
                
                // Show only one CONFIG byte number for the last, odd byte
                if (((Pk2.DevFile.PartsList[Pk2.ActivePart].ProgMemPanelBufs & 0x04) == 0x04)
                    && (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords <= editConfigWords))
                {
                    // cfgNames[editConfigWords-1] = "CONFIG" + (editConfigWords*2-1).ToString(); // alternative syntax
                    cfgNames[editConfigWords - 1] = string.Format("CONFIG{0:G}", editConfigWords * 2 - 1);
                }
                
                configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr;
                configIncrement = 2;
            }
            // 16-bit
            else if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue == 0xFFFFFF)
            {
                // PIC24F
                if (Pk2.FamilyIsPIC24FJ())
                {
                    for (int cw = 1; cw <= editConfigWords; cw ++)
                    {
                        cfgNames[editConfigWords - cw] = string.Format("CW{0:G}", cw);
                    }
                    configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr/2;
                    configIncrement = 2;
                }
                // PIC24EP, dsPIC33EP
                else if (Pk2.FamilyIsPIC24EP() || Pk2.FamilyIsdsPIC33EP())
                {
                    cfgNames[0] = "Reserved";
                    cfgNames[1] = "Reserved";
                    cfgNames[2] = "FICD";
                    cfgNames[3] = "FPOR";
                    cfgNames[4] = "FWDT";
                    cfgNames[5] = "FOSC";
                    cfgNames[6] = "FOSCSEL";
                    cfgNames[7] = "FGS";
                    cfgNames[8] = "Reserved";
                    configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr / 2;
                    configIncrement = 2;
                }
                // PIC24H, dsPIC33, dsPIC30 SMPS, PIC24F-KA-
                else if (Pk2.FamilyIsPIC24H() || Pk2.FamilyIsdsPIC33F() || Pk2.FamilyIsdsPIC30SMPS()
                        || (editConfigWords == 9))
                {
                    cfgNames[0] = "FBS";
                    cfgNames[1] = "FSS";
                    cfgNames[2] = "FGS";
                    cfgNames[3] = "FOSCSEL";
                    cfgNames[4] = "FOSC";
                    cfgNames[5] = "FWDT";
                    cfgNames[6] = "FPOR";
                    cfgNames[7] = "FICD";
                    cfgNames[8] = "FDS";
                    configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr / 2;
                    configIncrement = 2;
                }
                // dsPIC30
                else
                {
                    cfgNames[0] = "FOSC";
                    cfgNames[1] = "FWDT";
                    cfgNames[2] = "FBORPOR";
                    cfgNames[3] = "FBS";
                    cfgNames[4] = "FSS";
                    cfgNames[5] = "FGS";
                    cfgNames[6] = "FICD";
                    configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr/2;
                    configIncrement = 2;
                }
            }
            // PIC32
            else if (Pk2.FamilyIsPIC32())
            {
                //timijk: quick fix for PIC32MX1xx/2xx
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr != Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr)
                {
                    cfgNames[0] = "DEVCFG2L";
                    cfgNames[1] = "DEVCFG2H";
                    cfgNames[2] = "DEVCFG1L";
                    cfgNames[3] = "DEVCFG1H";
                    cfgNames[4] = "DEVCFG0L";
                    cfgNames[5] = "DEVCFG0H";
                }
                else
                {
                    cfgNames[0] = "    -   ";
                    cfgNames[1] = "DEVCFG3H";
                    cfgNames[2] = "DEVCFG2L";
                    cfgNames[3] = "DEVCFG2H";
                    cfgNames[4] = "DEVCFG1L";
                    cfgNames[5] = "DEVCFG1H";
                    cfgNames[6] = "DEVCFG0L";
                    cfgNames[7] = "DEVCFG0H";
                }

                configAddress = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr;
                configIncrement = 2;
            }

            for (int w = 0; w < K_MAXCONFIGS; w++)
            {
                if (w < editConfigWords)
                {
                    if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[w] == 0)
                    {
                    
                    }
                    configWords[w].name.Text = cfgNames[w];
                    configWords[w].addr.Text = string.Format("{0:X}", configAddress + (w * configIncrement));
                    ushort cword = (ushort)Pk2.DeviceBuffers.ConfigWords[w];
                    if (displayMask == 0)
                        cword &= (ushort)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[w];
                    else if (displayMask == 1)
                        cword |= (ushort)~Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[w];
                    cword &= (ushort)Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
                    configWords[w].value.Text = string.Format("{0:X4}", cword);
                    ushort mask = 1;
                    for (int b = 0; b < 16; b++)
                    {
                        if ((Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[w] & mask) > 0)
                        {
                            if ((Pk2.DeviceBuffers.ConfigWords[w] & mask) > 0)
                            {
                                configWords[w].bits[b].Text = "1";
                            }
                            else
                            {
                                configWords[w].bits[b].Text = "0";
                            }
                        }
                        else
                        {
                            configWords[w].bits[b].Text = "-";
                            configWords[w].bits[b].BackColor = System.Drawing.SystemColors.Control;
                            configWords[w].bits[b].Enabled = false;
                        }
                        mask <<= 1;
                    }
                    if (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[w] == 0)
                    {
                        configWords[w].configPanel.Enabled = false;
                    }
                }
                else
                {
                    configWords[w].configPanel.Visible = false;
                }
            }
        
        }

        private void textBox1_15_Click(object sender, EventArgs e)
        {
            int boxNum = (int)((TextBox)sender).Tag;
            int configNum = boxNum / 16;
            boxNum = boxNum % 16;
            uint bitMask = 1;
            bitMask <<= boxNum;
            //update box value
            if (configWords[configNum].bits[boxNum].Text == "1")
            {// change to 0
                configWords[configNum].bits[boxNum].Text = "0";
                Pk2.DeviceBuffers.ConfigWords[configNum] &= ~bitMask;
            }
            else
            { // change to 1
                configWords[configNum].bits[boxNum].Text = "1";
                Pk2.DeviceBuffers.ConfigWords[configNum] |= bitMask;
            }
            if (configWords[configNum].bits[boxNum].ForeColor == Color.Crimson) // red means edited
                configWords[configNum].bits[boxNum].ForeColor = System.Drawing.SystemColors.WindowText;
            else
                configWords[configNum].bits[boxNum].ForeColor = Color.Crimson;
            // update value column
            ushort cword = (ushort)Pk2.DeviceBuffers.ConfigWords[configNum];
            if (displayMask == 0)
                cword &= (ushort)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[configNum];
            else if (displayMask == 1)
                cword |= (ushort)~Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[configNum];
            cword &= (ushort)Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
            configWords[configNum].value.Text = string.Format("{0:X4}", cword);
            configWords[configNum].value.ForeColor = System.Drawing.SystemColors.ActiveCaption; // default blue
            for (int b = 0; b < 16; b++) // look for any edited bits in the word
            {
                if (configWords[configNum].bits[b].ForeColor == Color.Crimson)
                {
                    configWords[configNum].value.ForeColor = Color.Crimson; // red means edited
                    break;
                }
            }
        }

        private void DialogConfigEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool edits = false;

            for (int w = 0; w < editConfigWords; w++)
            {
                if (configWords[w].value.ForeColor == Color.Crimson)
                {
                    edits = true;
                    break;
                }
            }
            if (edits && !saveChanges)
            {
                if (MessageBox.Show
                    ("Are you sure you wish to exit\nwithout saving your Configuration edits?\n\nClick 'OK' to exit without saving your changes.",
                     "Exit without Saving?", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                // exit without saving- restore values.
                for (int cw = 0; cw < editConfigWords; cw++)
                {
                    Pk2.DeviceBuffers.ConfigWords[cw] = configSaves[cw];
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            saveChanges = false;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveChanges = true;
            FormPICkit2.ConfigsEdited = true;
            this.Close();
        }
    }
}