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
    public partial class FormMultiWinEEData : Form
    {

        public bool InitDone = false;

        private string dataFormat = "";
        private string addrFormat = "";
        private bool maxed = false;
        private bool progMemJustEdited = false;
        private int asciiBytes = 0;
        private int lastPart = 0;
        private int lastFam = 0;    
    
        public FormMultiWinEEData()
        {
            InitializeComponent();
        }

        public void InitMemDisplay(int viewMode)
        {
            // Init data grid
            dataGridProgramMemory.DefaultCellStyle.Font = new Font("Courier New", 9);
            comboBoxProgMemView.SelectedIndex = viewMode;

            ReCalcMultiWinMem();

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

        public void DisplayEETextOn(string displayText)
        {
            displayEEProgInfo.Text = displayText;
            displayEEProgInfo.Visible = true;
        }

        public void DisplayEETextOff()
        {
            displayEEProgInfo.Visible = false;
        }               

        public void ReCalcMultiWinMem()
        {   // call on 1) window init, 2) window resize, 3) new part, 4) Change view combo box
            uint memSize = Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem;

            if (memSize == 0)
                return;

            if (this.WindowState == FormWindowState.Minimized)
                return;                

            // first, calculate the address column size
            uint maxAddr = (memSize * Pk2.DevFile.Families[Pk2.GetActiveFamily()].EEMemAddressIncrement) - 1;
            int addrColWidth = 30; // FF max
            addrFormat = "{0:X2}";
            if (maxAddr > 0xFFF)
            {
                addrColWidth = 40;
                addrFormat = "{0:X4}";
            }            
            else if (maxAddr > 0xFF)
            {
                addrColWidth = 32; // FFF max
                addrFormat = "{0:X3}";
            }
            addrColWidth = (int)(addrColWidth * FormPICkit2.ScalefactW); // adjust for DPI

            // Figure data column size minimum
            uint blankValue = Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue;
            // col width min for PIC32 is 62
            int dataColWidth = 24;  //FF
            int asciiColWidth = 16;
            asciiBytes = 1;
            dataFormat = "{0:X2}";
            if (blankValue > 0xFFFF)
            { // FFFF
                dataColWidth = 36;
                asciiColWidth = 28;
                asciiBytes = 2;
                dataFormat = "{0:X4}";
            }
            else if (blankValue == 0xFFF)
            { // FFF baseline dataflash
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

            // clear selection
            if ((dataGridProgramMemory.FirstDisplayedCell != null) && !progMemJustEdited)
            {
                int currentRow = dataGridProgramMemory.FirstDisplayedCell.RowIndex;
                dataGridProgramMemory.MultiSelect = false;
                dataGridProgramMemory[0, currentRow].Selected = true;              // these 4 statements remove the "select" box
                dataGridProgramMemory[0, currentRow].Selected = false;
                dataGridProgramMemory.MultiSelect = true;
            }
            else if ((dataGridProgramMemory.FirstDisplayedCell == null) && (dataGridProgramMemory.RowCount > 0))
            { // remove select box when app first opened.
                dataGridProgramMemory.MultiSelect = false;
                dataGridProgramMemory[0, 0].Selected = true;              // these 4 statements remove the "select" box
                dataGridProgramMemory[0, 0].Selected = false;
                dataGridProgramMemory.MultiSelect = true;
            }
            progMemJustEdited = false;
            // format the display box
            dataGridProgramMemory.Rows.Clear();
            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                dataGridProgramMemory.ColumnCount = (2 * maxCols) + 1; // add in address column
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
                for (int i = maxCols + 1; i <= (2 * maxCols); i++)
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
            dataGridProgramMemory.RowCount = (int)numRows;
            // fill address column
            int rowAddrIncrement = maxCols * (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].EEMemAddressIncrement;
            for (int row = 0, addr = 0; row < numRows; row++)
            {
                dataGridProgramMemory[0, row].Value = string.Format(addrFormat, addr);
                dataGridProgramMemory[0, row].ReadOnly = true;
                dataGridProgramMemory[0, row].Style.BackColor = System.Drawing.SystemColors.ControlLight;
                addr += rowAddrIncrement;
            }

            updateDisplay();
        }

        public void UpdateMultiWinMem()
        {
            if ((lastPart != Pk2.ActivePart) || (lastFam != Pk2.GetActiveFamily()))
                ReCalcMultiWinMem();
            else
                updateDisplay();
        }

        private void updateDisplay()
        {
            int numRows = dataGridProgramMemory.RowCount - 1;
            int numCols = dataGridProgramMemory.ColumnCount - 1;
            int addressIncrement = (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].EEMemAddressIncrement;

            if (comboBoxProgMemView.SelectedIndex > 0)
            { // ascii view
                numCols /= 2;
            }

            for (int i = 0, idx = 0, address = 0; i < numRows; i++)
            {
                for (int j = 1; j <= numCols; j++)
                {
                    dataGridProgramMemory[j, i].Value = string.Format(dataFormat, Pk2.DeviceBuffers.EEPromMemory[idx++]);
                    dataGridProgramMemory[j, i].ToolTipText = string.Format(addrFormat, (address));
                    address += addressIncrement;
                }
            }
            int lastCol = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].EEMem % numCols;
            if (lastCol == 0)
                lastCol = numCols;
            int rowidx = numRows * numCols;
            // fill last row
            for (int j = 1; j <= numCols; j++)
            {
                if (j <= lastCol)
                {
                    dataGridProgramMemory[j, numRows].Value = string.Format(dataFormat, Pk2.DeviceBuffers.EEPromMemory[rowidx++]);
                    dataGridProgramMemory[j, numRows].ToolTipText = string.Format(addrFormat, (rowidx * addressIncrement));
                }
                else
                {
                    dataGridProgramMemory[j, numRows].Value = "";
                    dataGridProgramMemory[j, numRows].ToolTipText = "";
                    dataGridProgramMemory[j, numRows].ReadOnly = true;
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
                            dataGridProgramMemory[j, i].Value = UTIL.ConvertIntASCII((int)Pk2.DeviceBuffers.EEPromMemory[idx++], asciiBytes);
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
                            dataGridProgramMemory[j, numRows].Value = UTIL.ConvertIntASCII((int)Pk2.DeviceBuffers.EEPromMemory[rowidx++], asciiBytes);
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
                            dataGridProgramMemory[j, i].Value = UTIL.ConvertIntASCIIReverse((int)Pk2.DeviceBuffers.EEPromMemory[idx++], asciiBytes);
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
                            dataGridProgramMemory[j, numRows].Value = UTIL.ConvertIntASCIIReverse((int)Pk2.DeviceBuffers.EEPromMemory[rowidx++], asciiBytes);
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
                dataGridProgramMemory.MultiSelect = false;
                dataGridProgramMemory[0, 0].Selected = true;              // these 4 statements remove the "select" box
                dataGridProgramMemory[0, 0].Selected = false;
                dataGridProgramMemory.MultiSelect = true;
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

            uint blankValue = 0xFF;
            if (Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue > 0xFFFF)
            {
                blankValue = 0xFFFF; // dsPIC
            }
            else if ( Pk2.DevFile.Families[Pk2.GetActiveFamily()].BlankValue == 0xFFF)
            {
                blankValue = 0xFFF; // BL dataflash
            }

            Pk2.DeviceBuffers.EEPromMemory[((row * numColumns) + col - 1)] =
                        (uint)(value & blankValue);

            TellMainFormProgMemEdited();

            progMemJustEdited = true;
            TellMainFormUpdateGUI();
        }                                 
        
        
        public DelegateMultiEEMemClosed TellMainFormEEMemClosed;

        private void FormMultiWinEEData_FormClosing(object sender, FormClosingEventArgs e)
        {  
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                e.Cancel = true; // stop form from actually closing
                TellMainFormEEMemClosed();
                this.Hide();
            }
        }

        private void comboBoxProgMemView_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ReCalcMultiWinMem();
        }

        private void FormMultiWinEEData_ResizeEnd(object sender, EventArgs e)
        {
            ReCalcMultiWinMem();
        }

        private void FormMultiWinEEData_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                maxed = true;
                ReCalcMultiWinMem();
            } 
            else if (maxed)
            {
                maxed = false;
                ReCalcMultiWinMem();
            }
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