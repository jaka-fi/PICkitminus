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
    public partial class FormMultiWinProgMem : Form
    {
        public bool InitDone = false;
        
        private string dataFormat = "";
        private string addrFormat = "";
        private bool maxed = false;
        private bool progMemJustEdited = false;
        private int asciiBytes = 0;
        private int lastPart = 0;
        private int lastFam = 0;
    
        public FormMultiWinProgMem()
        {
            InitializeComponent();
        }
        
        public void InitProgMemDisplay(int viewMode)
        {
            // Init data grid
            dataGridProgramMemory.DefaultCellStyle.Font = new Font("Courier New", 9);
            comboBoxProgMemView.SelectedIndex = viewMode;

            //ReCalcMultiWinProgMem();            
            
            InitDone = true; // we've completed an init.
        }
        
        public int GetViewMode()
        {
            return comboBoxProgMemView.SelectedIndex;
        }
        
        public void DisplayDisable()
        {
            comboBoxProgMemView.Enabled = false;
            dataGridProgramMemory.Enabled = false;
            dataGridProgramMemory.ForeColor = System.Drawing.SystemColors.GrayText;
        }

        public void DisplayEnable()
        {
            comboBoxProgMemView.Enabled = true;
            dataGridProgramMemory.Enabled = true;
            dataGridProgramMemory.ForeColor = System.Drawing.SystemColors.WindowText;
        }      
        
        public void ReCalcMultiWinProgMem()
        {   // call on 1) window init, 2) window resize, 3) new part, 4) Change view combo box
            uint memSize = Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            if (memSize >= 0x80000)	// If device has large program memory (SPI FLASH devices)
            {
                memSize = 0x400;      // update only the visible rows
                dataGridProgramMemory.Enabled = false;
                dataGridProgramMemory.ForeColor = System.Drawing.SystemColors.GrayText;
            }
            else
            {
                dataGridProgramMemory.Enabled = true;
                dataGridProgramMemory.ForeColor = System.Drawing.SystemColors.WindowText;
            }


            if (memSize == 0)
                return;
                
            if (this.WindowState == FormWindowState.Minimized)
                return;
                

        
            // first, calculate the address column size
            uint blankValue = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
            // ASCII display disabled for PIC32 and parts with aux flash
            if (blankValue == 0xFFFFFFFF || Pk2.PartHasAuxFlash())
            {
                comboBoxProgMemView.SelectedIndex = 0;
                comboBoxProgMemView.Enabled = false;
            }
            else
            {
                comboBoxProgMemView.Enabled = true;
            }
            uint maxAddr = (memSize * Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement) - 1;
            int addrColWidth = 32; // FFF max
            addrFormat = "{0:X3}";
            if (blankValue == 0xFFFFFFFF)
            {// PIC32
                addrColWidth = 65; 
                addrFormat = "{0:X8}";
            }
            else if (Pk2.PartHasAuxFlash())
            {
                addrColWidth = 56; // FFFFFF max (and word 'Program' fits)
                addrFormat = "{0:X6}";
            }
            else if (maxAddr > 0xFFFF)
            {
                addrColWidth = 44; // FFFFF max
                addrFormat = "{0:X5}";
            }
            else if (maxAddr > 0xFFF)
            {
                addrColWidth = 38; // FFFF max
                addrFormat = "{0:X4}";
            }
            addrColWidth = (int)(addrColWidth * FormPICkit2.ScalefactW); // adjust for DPI
            
            // Figure data column size minimum
            int dataColWidth = 24;
            int asciiColWidth = 16;
            asciiBytes = 1;
            dataFormat = "{0:X2}";
            if (blankValue == 0xFFFFFFFF)
            { // PIC32
                dataColWidth = 65;
                asciiColWidth = 58;
                asciiBytes = 4;
                dataFormat = "{0:X8}";
            }            
            else if (blankValue > 0xFFFF)
            { // FFFFFF
                dataColWidth = 50;
                asciiColWidth = 43;
                asciiBytes = 3;
                dataFormat = "{0:X6}";
            }
            else if (blankValue > 0xFFF)
            { // FFFF or 3FFF
                dataColWidth = 36;
                asciiColWidth = 28;
                asciiBytes = 2;
                dataFormat = "{0:X4}";
            }
            else if (blankValue > 0xFF)
            { // FFF
                dataColWidth = 28;
                asciiColWidth = 28;
                asciiBytes = 2;
                dataFormat = "{0:X3}";
            }
            float colRatio = 1;
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                colRatio = (float)asciiColWidth / ((float)asciiColWidth + (float)dataColWidth);
                dataColWidth += asciiColWidth;
            }
            dataColWidth = (int)(dataColWidth * FormPICkit2.ScalefactW); // adjust for DPI 
            
            // calculate the number of data columns that will fit (must be mult of 2)
            // space left after address column and fudge factor for scroll bar:
            int dataSpace = dataGridProgramMemory.Size.Width - addrColWidth - (int)(20 * FormPICkit2.ScalefactW);  
            int maxCols = dataSpace / dataColWidth;
            for (int mult = 1; mult <= 256; mult *= 2)
            {
                if (mult > maxCols)
                {
                    maxCols = mult / 2;
                    break;
                }
            }
            if (maxCols > (int)memSize)
                maxCols = (int)memSize; // no point in having more columns than memory locations!
            // get the real width of the final column count
            dataColWidth = dataSpace / maxCols;
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                asciiColWidth = (int)(colRatio * (float)dataColWidth);
                dataColWidth -= asciiColWidth;
            }
        
            // format the display box
            dataGridProgramMemory.Rows.Clear();
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                dataGridProgramMemory.ColumnCount = (2*maxCols) + 1; // add in address column
            }
            else
            { // hex only
                dataGridProgramMemory.ColumnCount = maxCols + 1; // add in address column
            }
            // addr col
            dataGridProgramMemory.Columns[0].Width = addrColWidth;
            // data cols
            for (int i = 1; i <= maxCols; i++)
            {
                dataGridProgramMemory.Columns[i].Width = dataColWidth;
            }
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                for (int i = maxCols + 1; i <= (2* maxCols); i++)
                {
                    dataGridProgramMemory.Columns[i].Width = asciiColWidth;
                }            
            }
            // determine row count
            int numRows = (int)memSize / maxCols;
            if ((memSize % maxCols) > 0)
            { // mem size not an even divider, need an extra row
                numRows++;
            }
            if (blankValue == 0xFFFFFFFF || Pk2.PartHasAuxFlash())
            {
                numRows+= 2;
            }
            dataGridProgramMemory.RowCount = (int)numRows;
            // fill address column
            int rowAddrIncrement = maxCols * (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement;
            if (blankValue == 0xFFFFFFFF)
            {// PIC32
                int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
                int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
                progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
                progMemP32 /= maxCols;
                dataGridProgramMemory.ShowCellToolTips = false;
                // Program Flash addresses
                dataGridProgramMemory[0, 0].Value = "Program";
                dataGridProgramMemory[1, 0].Value = "Flash";
                for (int col = 0; col < dataGridProgramMemory.ColumnCount; col++)
                {
                    dataGridProgramMemory[col, 0].Style.BackColor = System.Drawing.SystemColors.ControlDark;
                    dataGridProgramMemory[col, 0].ReadOnly = true;
                }
                for (int row = 1, address = (int)KONST.P32_PROGRAM_FLASH_START_ADDR; row <= progMemP32; row++)
                {
                    dataGridProgramMemory[0, row].Value = string.Format(addrFormat, address);
                    dataGridProgramMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                    address += rowAddrIncrement;
                }
                // Boot Flash addresses
                dataGridProgramMemory[0, progMemP32 + 1].Value = "Boot";
                dataGridProgramMemory[1, progMemP32 + 1].Value = "Flash";
                for (int col = 0; col < dataGridProgramMemory.ColumnCount; col++)
                {
                    dataGridProgramMemory[col, progMemP32 + 1].Style.BackColor = System.Drawing.SystemColors.ControlDark;
                    dataGridProgramMemory[col, progMemP32 + 1].ReadOnly = true;
                }
                for (int row = progMemP32 + 2, address = (int)KONST.P32_BOOT_FLASH_START_ADDR; row < dataGridProgramMemory.RowCount; row++)
                {
                    dataGridProgramMemory[0, row].Value = string.Format(addrFormat, address);
                    dataGridProgramMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                    address += rowAddrIncrement;
                }
            }
            else if (Pk2.PartHasAuxFlash())
            {// PIC24/dsPIC33 with auxiliary flash
                int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
                int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
                progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
                progMemP32 /= maxCols;
                dataGridProgramMemory.ShowCellToolTips = false;
                // Program Flash addresses
                dataGridProgramMemory[0, 0].Value = "Program";
                dataGridProgramMemory[1, 0].Value = "Flash";
                for (int col = 0; col < dataGridProgramMemory.ColumnCount; col++)
                {
                    dataGridProgramMemory[col, 0].Style.BackColor = System.Drawing.SystemColors.ControlDark;
                    dataGridProgramMemory[col, 0].ReadOnly = true;
                }
                for (int row = 1, address = 0; row <= progMemP32; row++)
                {
                    dataGridProgramMemory[0, row].Value = string.Format(addrFormat, address);
                    dataGridProgramMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                    address += rowAddrIncrement;
                }
                // Boot Flash addresses
                dataGridProgramMemory[0, progMemP32 + 1].Value = "Aux.";
                dataGridProgramMemory[1, progMemP32 + 1].Value = "Flash";
                for (int col = 0; col < dataGridProgramMemory.ColumnCount; col++)
                {
                    dataGridProgramMemory[col, progMemP32 + 1].Style.BackColor = System.Drawing.SystemColors.ControlDark;
                    dataGridProgramMemory[col, progMemP32 + 1].ReadOnly = true;
                }
                for (int row = progMemP32 + 2, address = (int)Pk2.GetAuxFlashAddress() / (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement; row < dataGridProgramMemory.RowCount; row++)
                {
                    dataGridProgramMemory[0, row].Value = string.Format(addrFormat, address);
                    dataGridProgramMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                    address += rowAddrIncrement;
                }
            }
            else
            {
                dataGridProgramMemory.ShowCellToolTips = true;
                for (int row = 0, addr = 0; row < numRows; row++)
                {
                        dataGridProgramMemory[0, row].Value = string.Format(addrFormat, addr);
                        dataGridProgramMemory[0, row].ReadOnly = true;
                        dataGridProgramMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                        addr += rowAddrIncrement;
                }
            }

            updateDisplay();     
        }
        
        public void UpdateMultiWinProgMem(string dataSource)
        {
            if ((lastPart != Pk2.ActivePart) || (lastFam != Pk2.GetActiveFamily()))
            {
                lastPart = Pk2.ActivePart;
                lastFam = Pk2.GetActiveFamily();
                ReCalcMultiWinProgMem();
            }
            else
                updateDisplay();
            displayDataSource.Text = dataSource;
        }
        
        public string GetDataSource()
        {
            return displayDataSource.Text;
        }
        
        private void updateDisplay()
        {
            int numRows = dataGridProgramMemory.RowCount - 1;
            int numCols = dataGridProgramMemory.ColumnCount - 1;
            int addressIncrement = (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement;
            uint blankValue = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
            
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                numCols /= 2;
            }

            int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
            int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
            progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
            progMemP32 /= numCols;
            int lastCol = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem % numCols;
            if (lastCol == 0)
                lastCol = numCols;
            int rowidx = numRows * numCols;                
            if (blankValue == 0xFFFFFFFF || Pk2.PartHasAuxFlash())
            {
                // Program Flash
                int idx = 0;
                for (int row = 1; row <= progMemP32; row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        dataGridProgramMemory[col + 1, row].Value =
                            string.Format(dataFormat, Pk2.DeviceBuffers.ProgramMemory[idx++]);
                    }

                }
                // Boot Flash
                for (int row = progMemP32 + 2; row < (dataGridProgramMemory.RowCount - 1); row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        dataGridProgramMemory[col + 1, row].Value =
                            string.Format(dataFormat, Pk2.DeviceBuffers.ProgramMemory[idx++]);
                    }
                }
                lastCol = (int)bootMemP32 % numCols;
                if (lastCol == 0)
                    lastCol = numCols;
                // fill last row
                for (int j = 1; j <= numCols; j++)
                {
                    if (j <= lastCol)
                    {
                        dataGridProgramMemory[j, numRows].Value = string.Format(dataFormat, Pk2.DeviceBuffers.ProgramMemory[idx++]);
                    }
                    else
                    {
                        dataGridProgramMemory[j, numRows].Value = "";
                        dataGridProgramMemory[j, numRows].ReadOnly = true;
                    }
                }
            }
            else
            {
                for (int i = 0, idx = 0, address = 0; i < numRows; i++)
                {
                    for (int j = 1; j <= numCols; j++)
                    {
                        dataGridProgramMemory[j, i].Value = string.Format(dataFormat, Pk2.DeviceBuffers.ProgramMemory[idx++]);
                        dataGridProgramMemory[j, i].ToolTipText = string.Format(addrFormat, (address));
                        address += addressIncrement;
                    }
                }
                // fill last row
                for (int j = 1; j <= numCols; j++)
                {
                    if (j <= lastCol)
                    {
                        dataGridProgramMemory[j, numRows].Value = string.Format(dataFormat, Pk2.DeviceBuffers.ProgramMemory[rowidx]);
                        dataGridProgramMemory[j, numRows].ToolTipText = string.Format(addrFormat, (rowidx++ * addressIncrement));
                    }
                    else
                    {
                        dataGridProgramMemory[j, numRows].Value = "";
                        dataGridProgramMemory[j, numRows].ToolTipText = ""; 
                        dataGridProgramMemory[j, numRows].ReadOnly = true;
                    }
                }
            }
            
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                for (int c = numCols + 1; c <= (2 * numCols); c++)
                {// ASCII not editable
                    dataGridProgramMemory.Columns[c].ReadOnly = true;
                }                
                if (comboBoxProgMemView.SelectedIndex == 1)
                { // WORD view
                    for (int i = 0, idx = 0, address = 0; i < numRows; i++)
                    {
                        for (int j = numCols + 1; j <= (2 * numCols); j++)
                        {
                            dataGridProgramMemory[j, i].Value = UTIL.ConvertIntASCII((int)Pk2.DeviceBuffers.ProgramMemory[idx++], asciiBytes);
                            dataGridProgramMemory[j, i].ToolTipText = string.Format(addrFormat, (address));
                            address += addressIncrement;
                        }
                    }
                    // fill last row
                    rowidx = numRows * numCols;
                    for (int j = numCols + 1; j <= (2 * numCols); j++)
                    {
                        if (j <= (numCols + lastCol))
                        {
                            dataGridProgramMemory[j, numRows].Value = UTIL.ConvertIntASCII((int)Pk2.DeviceBuffers.ProgramMemory[rowidx++], asciiBytes);
                            dataGridProgramMemory[j, numRows].ToolTipText = string.Format(addrFormat, (rowidx * addressIncrement));
                        }
                        else
                        {
                            dataGridProgramMemory[j, numRows].Value = "";
                            dataGridProgramMemory[j, numRows].ToolTipText = "";
                            dataGridProgramMemory[j, numRows].ReadOnly = true;
                        }
                    }                              
                }
                else
                { // BYTE view
                    for (int i = 0, idx = 0, address = 0; i < numRows; i++)
                    {
                        for (int j = numCols + 1; j <= (2 * numCols); j++)
                        {
                            dataGridProgramMemory[j, i].Value = UTIL.ConvertIntASCIIReverse((int)Pk2.DeviceBuffers.ProgramMemory[idx++], asciiBytes);
                            dataGridProgramMemory[j, i].ToolTipText = string.Format(addrFormat, (address));
                            address += addressIncrement;
                        }
                    }
                    // fill last row
                    rowidx = numRows * numCols;
                    for (int j = numCols + 1; j <= (2 * numCols); j++)
                    {
                        if (j <= (numCols + lastCol))
                        {
                            dataGridProgramMemory[j, numRows].Value = UTIL.ConvertIntASCIIReverse((int)Pk2.DeviceBuffers.ProgramMemory[rowidx++], asciiBytes);
                            dataGridProgramMemory[j, numRows].ToolTipText = string.Format(addrFormat, (rowidx * addressIncrement));
                        }
                        else
                        {
                            dataGridProgramMemory[j, numRows].Value = "";
                            dataGridProgramMemory[j, numRows].ToolTipText = "";
                            dataGridProgramMemory[j, numRows].ReadOnly = true;
                        }
                    }                  
                }
            }

            if ((dataGridProgramMemory.FirstDisplayedCell != null) && !progMemJustEdited)
            {
                int currentRow = dataGridProgramMemory.FirstDisplayedCell.RowIndex;
                dataGridProgramMemory.MultiSelect = false;
                dataGridProgramMemory[0, currentRow].Selected = true;              // these 4 statements remove the "select" box
                dataGridProgramMemory[0, currentRow].Selected = false;
                dataGridProgramMemory.MultiSelect = true;
            }
            else if (dataGridProgramMemory.FirstDisplayedCell == null)
            { // remove select box when app first opened.
                if (dataGridProgramMemory.RowCount > 0)
                {
                    dataGridProgramMemory.MultiSelect = false;
                    dataGridProgramMemory[0, 0].Selected = true;              // these 4 statements remove the "select" box
                    dataGridProgramMemory[0, 0].Selected = false;
                    dataGridProgramMemory.MultiSelect = true;
                }
            }
            progMemJustEdited = false;
        }

        public DelegateMemEdited TellMainFormProgMemEdited;
        public DelegateUpdateGUI TellMainFormUpdateGUI;

        private void progMemEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            string editText = "0x" + dataGridProgramMemory[col, row].FormattedValue.ToString();
            int value = 0;
            try
            {
                value = UTIL.Convert_Value_To_Int(editText);
            }
            catch
            {
                value = 0;
            }
            int numColumns = dataGridProgramMemory.ColumnCount - 1;
            if (comboBoxProgMemView.SelectedIndex >= 1) // ascii view
            {
                numColumns /= 2;
            }
            
            int index = ((row * numColumns) + col - 1);
            
            if (Pk2.FamilyIsPIC32() || Pk2.PartHasAuxFlash())
            {
                int progMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ProgramMem;
                int bootMemP32 = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].BootFlash;
                progMemP32 -= bootMemP32; // boot flash at upper end of prog mem.
                
                index-= numColumns; // first row has "Program Flash" text
                
                if (index > progMemP32)
                {
                    index -= numColumns; // subtract row with "Boot Flash" text  
                }
            }

            Pk2.DeviceBuffers.ProgramMemory[index] =
                        (uint)(value & Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue);

            TellMainFormProgMemEdited();

            progMemJustEdited = true;
            TellMainFormUpdateGUI(); 
        }        

        public DelegateMultiProgMemClosed TellMainFormProgMemClosed;
        
        private void FormMultiWinProgMem_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {        
                e.Cancel = true; // stop form from actually closing
                TellMainFormProgMemClosed();
                this.Hide();
            }
            
        }

        private void FormMultiWinProgMem_ResizeEnd(object sender, EventArgs e)
        {
            ReCalcMultiWinProgMem();
        }

        private void FormMultiWinProgMem_Resize(object sender, EventArgs e)
        {        
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxed = true;
                ReCalcMultiWinProgMem();
            } 
            else if (maxed)
            {
                maxed = false;
                ReCalcMultiWinProgMem();
            }
        }

        private void comboBoxProgMemView_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ReCalcMultiWinProgMem();
        }

        private void toolStripMenuItemContextSelectAll_Click(object sender, EventArgs e)
        {
            dataGridProgramMemory.SelectAll();
        }

        private void toolStripMenuItemContextCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.dataGridProgramMemory.GetClipboardContent());
        }

        private void dataGridProgramMemory_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                dataGridProgramMemory.Focus();
        }
    }
}