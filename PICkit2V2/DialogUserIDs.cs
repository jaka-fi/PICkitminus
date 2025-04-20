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
    public partial class DialogUserIDs : Form
    {
        public static bool IDMemOpen = false;
    
        public DialogUserIDs()
        {
            InitializeComponent();
            IDMemOpen = true;
            
            // init datagrid
            dataGridViewIDMem.DefaultCellStyle.Font = new Font("Courier New", 9);
            UpdateIDMemoryGrid();
        }
        
        public void UpdateIDMemoryGrid()
        {
            const int cols = 4;
            //const int colWidth = 53;
            int colWidth = (int) (53 * FormPICkit2.ScalefactW);

            dataGridViewIDMem.ColumnCount = 4;
            for (int column = 0; column < dataGridViewIDMem.ColumnCount; column++)
            {
                dataGridViewIDMem.Columns[column].Width = colWidth;
            }
            
            int rows = Pk2.DeviceBuffers.UserIDs.Length / cols;
            dataGridViewIDMem.RowCount = rows;
            int row = 0;
            int col = 0;

            for (int idx = 0; idx < Pk2.DeviceBuffers.UserIDs.Length; idx++)
            {
                dataGridViewIDMem[col, row].Value = string.Format("{0:X6}", Pk2.DeviceBuffers.UserIDs[idx]);
                col++;
                if (col >= cols)
                {
                    col = 0;
                    row++;
                }
            }

            dataGridViewIDMem[0, 0].Selected = true;              // these 2 statements remove the "select" box
            dataGridViewIDMem[0, 0].Selected = false;  
            
        }

        private void DialogUserIDs_FormClosing(object sender, FormClosingEventArgs e)
        {
            IDMemOpen = false;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        
    }
}