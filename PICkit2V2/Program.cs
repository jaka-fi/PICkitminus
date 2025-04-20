using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PICkit2V2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();         // Comment out to allow solid progress bar and tan menu
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPICkit2());
        }
    }
}