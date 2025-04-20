using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PICkit2V2
{
    public partial class DialogTrigger : Form
    {
        public DialogTrigger()
        {
            InitializeComponent();
            this.Size = new Size(this.Size.Width, (int)(FormPICkit2.ScalefactH * this.Size.Height));
        }
    }
}