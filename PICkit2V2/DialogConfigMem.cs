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
    public partial class DialogConfigMem : Form
    {
        public static bool ConfigMemOpen = false;
    
        public DialogConfigMem()
        {
            InitializeComponent();
            ConfigMemOpen = true;
            
            // init datagrid
            dataGridViewConfigMem.DefaultCellStyle.Font = new Font("Courier New", 9);
            UpdateConfigMemoryGrid();
        }
        
        public void UpdateConfigMemoryGrid()
        {
            const int cols = 3;
            //const int colWidth = 53;
            int colWidth = (int) (71 * FormPICkit2.ScalefactW);

            dataGridViewConfigMem.ColumnCount = cols;
            for (int column = 0; column < dataGridViewConfigMem.ColumnCount; column++)
            {
                dataGridViewConfigMem.Columns[column].Width = colWidth;
            }

            int rows = Pk2.DeviceBuffers.ConfigWords.Length;
            //int rows = 8 / cols;
            dataGridViewConfigMem.RowCount = rows;
            int row = 0;
            int col = 0;
            int configBase = (int)Pk2.DevFile.PartsList[Pk2.ActivePart].ConfigAddr;
            int configIncrement = (int)Pk2.DevFile.Families[Pk2.GetActiveFamily()].AddressIncrement;
            
            for (int idx = 0; idx < Pk2.DeviceBuffers.ConfigWords.Length; idx++)
            //for (int idx = 0; idx < 8; idx++)
            {
                dataGridViewConfigMem[1, row].Value = string.Format("{0:X6}", configBase + idx*configIncrement);
                dataGridViewConfigMem[2, row].Value = string.Format("{0:X8}", Pk2.DeviceBuffers.ConfigWords[idx]);
                row++;
            }

            dataGridViewConfigMem[0, 0].Selected = true;              // these 2 statements remove the "select" box
            dataGridViewConfigMem[0, 0].Selected = false;  
            
        }


        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DialogConfigMem_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfigMemOpen = false;
        }
    }
}