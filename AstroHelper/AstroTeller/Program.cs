using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AstroTeller
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (GlobalItems.MdiWindow == null)
                {
                    GlobalItems.MdiWindow = new MainForm();
                    Application.Run(GlobalItems.MdiWindow);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
