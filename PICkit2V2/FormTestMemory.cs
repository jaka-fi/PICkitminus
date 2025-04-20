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
    public partial class FormTestMemory : Form
    {
        public static uint[] TestMemory = new uint[1024];  // limit to 1024 for now.
        public static bool ReWriteCalWords = false;

        private bool testMemJustEdited = false;
        private int lastPart = 0;
        private int lastFamily = 0;
    
        public FormTestMemory()
        {
            InitializeComponent();
            initTestMemoryGUI();
            ClearTestMemory();

            UpdateTestMemForm();
            UpdateTestMemoryGrid();

            FormPICkit2.TestMemoryOpen = true;
        }
        
        public void ReadTestMemory()
        {
            byte[] upload_buffer = new byte[KONST.UploadBufferSize];

            Pk2.RunScript(KONST.PROG_ENTRY, 1);
            int bytesPerWord = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int scriptRunsToFillUpload = KONST.UploadBufferSize /
                (Pk2.DevFile.PartsList[Pk2.ActivePart].TestMemoryRdWords * bytesPerWord);
            int wordsPerLoop = scriptRunsToFillUpload * Pk2.DevFile.PartsList[Pk2.ActivePart].TestMemoryRdWords;
            int wordsRead = 0;

            prepTestMem();

            do
            {
                //Pk2.RunScriptUploadNoLen2(KONST.TESTMEM_RD, scriptRunsToFillUpload);
                Pk2.RunScriptUploadNoLen(KONST.TESTMEM_RD, scriptRunsToFillUpload);
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, 0, KONST.USB_REPORTLENGTH);
                //Pk2.GetUpload();
                Pk2.UploadDataNoLen();
                Array.Copy(Pk2.Usb_read_array, 1, upload_buffer, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
                int uploadIndex = 0;
                for (int word = 0; word < wordsPerLoop; word++)
                {
                    int bite = 0;
                    uint memWord = (uint)upload_buffer[uploadIndex + bite++];
                    if (bite < bytesPerWord)
                    {
                        memWord |= (uint)upload_buffer[uploadIndex + bite++] << 8;
                    }
                    if (bite < bytesPerWord)
                    {
                        memWord |= (uint)upload_buffer[uploadIndex + bite++] << 16;
                    }
                    if (bite < bytesPerWord)
                    {
                        memWord |= (uint)upload_buffer[uploadIndex + bite++] << 24;
                    }
                    uploadIndex += bite;
                    //$$$Bequest333 and JAKA
                    if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].FamilyName == "Midrange/1.8V Min MSB1st" ||
                        Pk2.DevFile.Families[Pk2.GetActiveFamily()].FamilyName == "PIC18/PIC18F MSB1st")
                    {
                        memWord = reverse16Bits(memWord);
                    }
                    //$$$
                    // shift if necessary
                    if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemShift > 0)
                    {
                        memWord = (memWord >> 1) & Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
                    }
                    TestMemory[wordsRead++] = memWord;                
                }
            } while (wordsRead < FormPICkit2.TestMemoryWords);

            Pk2.RunScript(KONST.PROG_EXIT, 1);
        }
        
        public bool HexImportExportTM()
        {
            return (checkBoxTestMemImportExport.Enabled && checkBoxTestMemImportExport.Checked);
        }

        //$$$Bequest333
        private static uint reverse16Bits(uint v)
        {
            v = ((v & 0x0000ff00) >> 8) | ((v & 0x000000ff) << 8);
            v = ((v & 0x0000f0f0) >> 4) | ((v & 0x00000f0f) << 4);
            v = ((v & 0x0000cccc) >> 2) | ((v & 0x00003333) << 2);
            v = ((v & 0x0000aaaa) >> 1) | ((v & 0x00005555) << 1);
            return v;
        }
        //$$$


        private void prepTestMem()
        {
            // Send pre-requisite scripts
            if ((Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x4000) || (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x10000))
            { // midrange = Load Config command
                byte[] prepscript = new byte[7];
                prepscript[0] = KONST._WRITE_BITS_LITERAL;
                prepscript[1] = 0x06;
                prepscript[2] = 0x00;
                prepscript[3] = KONST._WRITE_BYTE_LITERAL;
                prepscript[4] = 0;
                prepscript[5] = KONST._WRITE_BYTE_LITERAL;
                prepscript[6] = 0;
                Pk2.SendScript(prepscript);
            }
            else if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x800)
            { // baseline
                int tmStart = (getTestMemAddress() / 4) - 1;
                byte[] prepscript = new byte[18];
                prepscript[0] = KONST._CONST_WRITE_DL;      // put # loops in buffer for LOOPBUFFER
                prepscript[1] = (byte)(tmStart & 0xFF);
                prepscript[2] = KONST._CONST_WRITE_DL;
                prepscript[3] = (byte)((tmStart >> 8) & 0xFF);                        
                prepscript[4] = KONST._WRITE_BITS_LITERAL;
                prepscript[5] = 0x06;
                prepscript[6] = 0x06;
                prepscript[7] = KONST._WRITE_BITS_LITERAL;
                prepscript[8] = 0x06;
                prepscript[9] = 0x06;
                prepscript[10] = KONST._WRITE_BITS_LITERAL;
                prepscript[11] = 0x06;
                prepscript[12] = 0x06;
                prepscript[13] = KONST._WRITE_BITS_LITERAL;
                prepscript[14] = 0x06;
                prepscript[15] = 0x06;   
                prepscript[16] = KONST._LOOPBUFFER;
                prepscript[17] = 12;
                Pk2.SendScript(prepscript);            
            }
            else if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x200000)
            { // PIC18F - Set address pointer to 0x200000
                byte[] prepscript = new byte[18];
                prepscript[0] = KONST._COREINST18;
                prepscript[1] = 0x20;
                prepscript[2] = 0x0E;
                prepscript[3] = KONST._COREINST18;
                prepscript[4] = 0xF8;
                prepscript[5] = 0x6E;
                prepscript[6] = KONST._COREINST18;
                prepscript[7] = 0x00;
                prepscript[8] = 0x0E;
                prepscript[9] = KONST._COREINST18;
                prepscript[10] = 0xF7;
                prepscript[11] = 0x6E;                
                prepscript[12] = KONST._COREINST18;
                prepscript[13] = 0x00;
                prepscript[14] = 0x0E;
                prepscript[15] = KONST._COREINST18;
                prepscript[16] = 0xF6;
                prepscript[17] = 0x6E;                
                Pk2.SendScript(prepscript);
            }
        }        
        
        private void initTestMemoryGUI()
        {           
            // Init the Program Memory grid.       
            // Set default Cell font
            dataGridTestMemory.DefaultCellStyle.Font = new Font("Courier New", 9);
            dataGridTestMemory.ColumnCount = 9;
            dataGridTestMemory.RowCount = 512;
            dataGridTestMemory[0, 0].Selected = true;              // these 2 statements remove the "select" box
            dataGridTestMemory[0, 0].Selected = false;              
            //dataGridTestMemory.Columns[0].Width = 59; // address column
            dataGridTestMemory.Columns[0].Width = (int)(59 * FormPICkit2.ScalefactW); // address column
            dataGridTestMemory.Columns[0].ReadOnly = true;
/*            for (int column = 1; column < 9; column++)
            {
                dataGridTestMemory.Columns[column].Width = 53; // data columns
            }
            for (int row = 0; row < 32; row++)
            {
                dataGridTestMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                dataGridTestMemory[0, row].Value = string.Format("{0:X5}", row * 8);
            }
*/
            if (FormPICkit2.TestMemoryImportExport)
            {
                checkBoxTestMemImportExport.Checked = true;
            }

        }    
        
        public void UpdateTestMemForm()
        {
            // Test Memory Words
            textBoxTestMemSize.Text = FormPICkit2.TestMemoryWords.ToString();
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart > 0)
            { // baseline, midrange, PIC18F
                textBoxBaselineConfig.Enabled = true;
                textBoxTestMemSize.Enabled = true;
                checkBoxTestMemImportExport.Enabled = true;
                labelNotSupported.Visible = false;
            }
            else
            {
                textBoxBaselineConfig.Enabled = false;
                textBoxTestMemSize.Enabled = false;
                checkBoxTestMemImportExport.Enabled = false;
                dataGridTestMemory.Enabled = false;
                labelNotSupported.Visible = true;
            }
        }
        
        public void ClearTestMemory()
        {
            for (int i = 0; i < TestMemory.Length; i++)
            {
                TestMemory[i] = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
            }
        }
        
        
        public void UpdateTestMemoryGrid()
        {        
            bool userIDInTestMem = false;
            int userIDIndex = 0;
            int testMemStart = getTestMemAddress() * Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
            int userIDAddr = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr;
            
            // reset buffers if new family or part.
            if ((Pk2.ActivePart != lastPart) || (Pk2.GetActiveFamily() != lastFamily))
            {
                ClearTestMemory();
                lastPart = Pk2.ActivePart;
                lastFamily = Pk2.GetActiveFamily();
            }

            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x800)
            { // baseline
                userIDAddr = testMemStart;
            }
            
            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDWords > 0)
               && (userIDAddr >= testMemStart) && (userIDAddr < (testMemStart + FormPICkit2.TestMemoryWords)))
               { // if user IDs are in test memory 
                    userIDInTestMem = true;
                    userIDIndex = (int)(userIDAddr - testMemStart)
                                    / Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
               }
            bool cfgWordsInTestMem = false;
            int cfgWordsIndex = 0;
            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords > 0)
               && (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr >= testMemStart)
               && (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr < (testMemStart + FormPICkit2.TestMemoryWords)))
               { // if user IDs are in test memory 
                    cfgWordsInTestMem = true;
                    cfgWordsIndex = (int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr - testMemStart)
                                    / Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
               }               
               
            // Update Test Memory display          
            //      Set address columns first       
            //      Set # columns based on blank memory size.
            int dataColumns, columnWidth, columnCount;
            columnCount = 9;
            //dataGridTestMemory.Columns[0].Width = 51; // address column
            dataGridTestMemory.Columns[0].Width = (int)(51 * FormPICkit2.ScalefactW); // address column
            dataColumns = 8;
            //columnWidth = 35;
            columnWidth = (int)(35 * FormPICkit2.ScalefactW);
            
            int startAddress = getTestMemAddress();
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x200000)
            {
                startAddress *=2;
            }

            if (dataGridTestMemory.ColumnCount != columnCount)
            {
                dataGridTestMemory.Rows.Clear();
                dataGridTestMemory.ColumnCount = columnCount;
            }

            for (int column = 1; column < dataGridTestMemory.ColumnCount; column++)
            {
                dataGridTestMemory.Columns[column].Width = columnWidth; // data columns
            }
            
            int addressIncrement = (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement;
            int rowAddressIncrement;
            int hexColumns, rowCount;
//            if (comboBoxProgMemView.SelectedIndex == 0) // hex view
//            {
                hexColumns = dataColumns;
                rowCount = (int)FormPICkit2.TestMemoryWords / hexColumns; 
                rowAddressIncrement = addressIncrement * dataColumns;
//            }
//            else
//            {
//                hexColumns = dataColumns / 2;
//                rowCount = (int)TestMemoryWords / hexColumns; 
//                rowAddressIncrement = addressIncrement * (dataColumns / 2);
//            }
            if (dataGridTestMemory.RowCount != rowCount)
            {
                dataGridTestMemory.Rows.Clear();
                dataGridTestMemory.RowCount = rowCount;
            }
            
            int maxAddress = dataGridTestMemory.RowCount * rowAddressIncrement - 1;
            String addressFormat = "{0:X3}";
            if (maxAddress > 0xFFFF)
            {
                addressFormat = "{0:X5}";
            }
            else if (maxAddress > 0xFFF)
            {
                addressFormat = "{0:X4}";
            }
            for (int row = 0, address = 0; row < dataGridTestMemory.RowCount; row++)
            {
                dataGridTestMemory[0, row].Value = string.Format(addressFormat, startAddress + address);
                dataGridTestMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                address += rowAddressIncrement;
            } 
            //      Now fill data
            string dataFormat = "{0:X3}";
            //int asciiBytes = 2;
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFF)
            {
                dataFormat = "{0:X4}";
            }
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFF)
            {
                dataFormat = "{0:X6}";
            //    asciiBytes = 3;
            }
            for (int col = 0; col < hexColumns; col++)
            { // only allow UserIDs and Config words to be edited.
                dataGridTestMemory.Columns[col + 1].ReadOnly = true;
            }               
            for (int row = 0, idx = 0; row < dataGridTestMemory.RowCount; row++)
            {
                for (int col = 0; col < hexColumns; col++)
                {
                    if (userIDInTestMem && (idx >= userIDIndex)
                       && (idx < (userIDIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDWords)))
                    {
                        TestMemory[idx] = Pk2.DeviceBuffers.UserIDs[idx - userIDIndex];
                        dataGridTestMemory[col + 1, row].ToolTipText =
                            string.Format(addressFormat, startAddress + (idx * addressIncrement));
                        dataGridTestMemory[col + 1, row].Value =
                             string.Format(dataFormat, Pk2.DeviceBuffers.UserIDs[idx++ - userIDIndex]);  
                        dataGridTestMemory[col + 1, row].Style.BackColor = Color.LightSteelBlue;   
                        dataGridTestMemory[col + 1, row].ReadOnly = false;           
                    }
                    else if (cfgWordsInTestMem && (idx >= cfgWordsIndex)
                       && (idx < (cfgWordsIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords)))
                    {
                        TestMemory[idx] = Pk2.DeviceBuffers.ConfigWords[idx - cfgWordsIndex];
                        dataGridTestMemory[col + 1, row].ToolTipText =
                            string.Format(addressFormat, startAddress + (idx * addressIncrement));
                        dataGridTestMemory[col + 1, row].Value =
                             string.Format(dataFormat, Pk2.DeviceBuffers.ConfigWords[idx++ - cfgWordsIndex]);
                        dataGridTestMemory[col + 1, row].Style.BackColor = Color.LightSalmon;
                        dataGridTestMemory[col + 1, row].ReadOnly = false;
                    }     
                    else if ((Pk2.DevFile.PartsList[Pk2.ActivePart].CalibrationWords > 0)
                            && (idx >= (cfgWordsIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords))
                            && (idx < (cfgWordsIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords
                                    + Pk2.DevFile.PartsList[Pk2.ActivePart].CalibrationWords)))
                    {
                        dataGridTestMemory[col + 1, row].ToolTipText =
                            string.Format(addressFormat, startAddress + (idx * addressIncrement));
                        dataGridTestMemory[col + 1, row].Value =
                            string.Format(dataFormat, TestMemory[idx++]);
                        dataGridTestMemory[col + 1, row].Style.BackColor = Color.Gold;
                        dataGridTestMemory[col + 1, row].ReadOnly = false;                 
                    }       
                    else
                    {
                        dataGridTestMemory[col + 1, row].ToolTipText =
                            string.Format(addressFormat, startAddress + (idx * addressIncrement));
                        dataGridTestMemory[col + 1, row].Value =
                            string.Format(dataFormat, TestMemory[idx++]);  
                        dataGridTestMemory[col + 1, row].Style.BackColor = System.Drawing.SystemColors.Window;    
                    }                                      
                        
                }
            
            }
            //      Fill ASCII if selected
            /*            if (comboBoxProgMemView.SelectedIndex == 1)
                        {
                            for (int col = 0; col < hexColumns; col++)
                            { // ascii not editable
                                dataGridTestMemory.Columns[col + hexColumns + 1].ReadOnly = true;
                            }
                            for (int row = 0, idx = 0; row < dataGridTestMemory.RowCount; row++)
                            {
                                for (int col = 0; col < hexColumns; col++)
                                {
                                    dataGridTestMemory[col + hexColumns + 1, row].ToolTipText =
                                        string.Format(addressFormat, (idx * addressIncrement));                        
                                    dataGridTestMemory[col + hexColumns + 1, row].Value = 
                                        UTIL.ConvertIntASCII((int)testMemory[idx++], asciiBytes); 
                                }

                            }
                        }
            */
            if ((dataGridTestMemory.FirstDisplayedCell != null) && !testMemJustEdited)
            {
                //currentCol = dataGridProgramMemory.FirstDisplayedCell.ColumnIndex;
                int currentRow = dataGridTestMemory.FirstDisplayedCell.RowIndex;
                dataGridTestMemory[0, currentRow].Selected = true;              // these 2 statements remove the "select" box
                dataGridTestMemory[0, currentRow].Selected = false;                        
            }
            else if (dataGridTestMemory.FirstDisplayedCell == null)
            { // remove select box when app first opened.
                dataGridTestMemory[0, 0].Selected = true;              // these 2 statements remove the "select" box
                dataGridTestMemory[0, 0].Selected = false;                   
            }
            testMemJustEdited = false;

            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x800)
            { // baseline
                labelBLConfig.Visible = true;
                textBoxBaselineConfig.Visible = true;
                textBoxBaselineConfig.Text = string.Format("{0:X4}", Pk2.DeviceBuffers.ConfigWords[0]);
            }
            else
            {
                labelBLConfig.Visible = false;
                textBoxBaselineConfig.Visible = false;
            }

            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x4000)
            { // midrange
                labelCalWarning.Visible = true;
                buttonWriteCalWords.Visible = true;
                buttonWriteCalWords.Enabled = (Pk2.DevFile.PartsList[Pk2.ActivePart].CalibrationWords > 0);
            }
            else
            {
                labelCalWarning.Visible = false;
                buttonWriteCalWords.Visible = false;
            }
                     
        }

        private void FormTestMemory_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormPICkit2.TestMemoryOpen = false;
        }
        
        private int getTestMemAddress()
        {
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x800)
            { // baseline
                int tMemAddr = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
                if (Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem > 0)
                { // baseline with dataflash
                    // test mem starts after data flash
                    tMemAddr += Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem;
                }
                return tMemAddr;
            }
            else
            {
                return (int)(Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart
                                / Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes);
            }
        }

        private void textBoxTestMemSize_Leave(object sender, EventArgs e)
        {
            memSizeEdit();
        }

        private void textBoxTestMemSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                memSizeEdit();
            }
        }
        
        private void memSizeEdit()
        {
            labelTestMemSize8.Visible = false;
            try
            {
                string editText  = textBoxTestMemSize.Text;
                if (textBoxTestMemSize.Text.Length > 1)
                {
                    if (textBoxTestMemSize.Text.Substring(0, 2) == "0x")
                    {
                        editText = textBoxTestMemSize.Text;
                    }
                    else if (textBoxTestMemSize.Text.Substring(0, 1) == "x")
                    {
                        editText = "0" + textBoxTestMemSize.Text;
                    }                    
                }
                FormPICkit2.TestMemoryWords = UTIL.Convert_Value_To_Int(editText);
                if (FormPICkit2.TestMemoryWords > 1024)
                {
                    FormPICkit2.TestMemoryWords = 1024;
                }
                else if (FormPICkit2.TestMemoryWords < 16)
                {
                    FormPICkit2.TestMemoryWords = 16;
                }
                else if ((FormPICkit2.TestMemoryWords % 16) != 0)
                { // must be multiple of 16 - otherwise grid view handling & hex exporting gets complicated
                    FormPICkit2.TestMemoryWords = ((FormPICkit2.TestMemoryWords / 16) + 1) * 16;
                    labelTestMemSize8.Visible = true;
                }
            }
            catch
            {   // don't do anything
            
            }
            UpdateTestMemForm();
            UpdateTestMemoryGrid();
        }

        public DelegateUpdateGUI UpdateMainFormGUI;
        
        private void dataGridTestMemory_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            string editText = "0x" + dataGridTestMemory[col, row].FormattedValue.ToString();
            int value = UTIL.Convert_Value_To_Int(editText);
            int numColumns = dataGridTestMemory.ColumnCount - 1;
            int testMemAddress = (row * numColumns) + col - 1;
            int testMemStart = getTestMemAddress() * Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
            int userIDAddr = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDAddr;
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x800)
            { // baseline
                userIDAddr = testMemStart;
            }            

            TestMemory[testMemAddress] =
                        (uint)(value & Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue);

            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDWords > 0)
               && (userIDAddr >= testMemStart) && (userIDAddr < (testMemStart + FormPICkit2.TestMemoryWords)))
            { // if user IDs are in test memory 
                int userIDIndex = (int)(userIDAddr - testMemStart)
                                / Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
                if ((testMemAddress >= userIDIndex) && (testMemAddress < (userIDIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].UserIDWords)))
                {
                    Pk2.DeviceBuffers.UserIDs[testMemAddress - userIDIndex] = 
                        (uint)(value & Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue);
                    if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].TestMemoryStart == 0x200000)
                    { // 18F
                        Pk2.DeviceBuffers.UserIDs[testMemAddress - userIDIndex] &= 0xFF;
                    }
                }
            }

            if ((Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords > 0) &&
               (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr >= testMemStart) &&
               (Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr < (testMemStart + FormPICkit2.TestMemoryWords)))
            { // if config words are in test memory 
                int cfgWordsIndex = (int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr - testMemStart)
                                / Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
                if ((testMemAddress >= cfgWordsIndex) && (testMemAddress < (cfgWordsIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords)))
                {
                    Pk2.DeviceBuffers.ConfigWords[testMemAddress - cfgWordsIndex] =
                        (uint)(value & Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigMasks[testMemAddress - cfgWordsIndex]);
                }
            }                                   

            testMemJustEdited = true;
            UpdateMainFormGUI();
        }

        private void textBoxBaselineConfig_Leave(object sender, EventArgs e)
        {
            baselineConfigEdit();
        }

        private void textBoxBaselineConfig_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                baselineConfigEdit();
            }
        }
        
        private void baselineConfigEdit()
        {
            string editText = "0x" + textBoxBaselineConfig.Text;
            int value = UTIL.Convert_Value_To_Int(editText);   
            Pk2.DeviceBuffers.ConfigWords[0] = (uint)value;
            UpdateTestMemoryGrid();
            UpdateMainFormGUI();
        }

        private void checkBoxTestMemImportExport_CheckedChanged(object sender, EventArgs e)
        {
            FormPICkit2.TestMemoryImportExport = checkBoxTestMemImportExport.Checked;
        }

        private void buttonClearTestMem_Click(object sender, EventArgs e)
        {
            ClearTestMemory();
            UpdateTestMemoryGrid();
        }

        public DelegateWriteCal CallMainFormEraseWrCal;

        private void buttonWriteCalWords_Click(object sender, EventArgs e)
        {
            uint[] cals = new uint[Pk2.DevFile.PartsList[Pk2.ActivePart].CalibrationWords];
            
            int testMemStart = getTestMemAddress() * Pk2.DevFile.Families[Pk2.GetActiveFamily()].ProgMemHexBytes;
            int cfgWordsIndex = (int)(Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr - testMemStart)
                                    / Pk2.DevFile.Families[Pk2.GetActiveFamily()].BytesPerLocation;
            int calWordsIndex = cfgWordsIndex + Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigWords;
            
            for (int i = 0; i < cals.Length; i++)
            {
                cals[i] = TestMemory[calWordsIndex + i];
            }
            
            CallMainFormEraseWrCal(cals);
        }
    }
}