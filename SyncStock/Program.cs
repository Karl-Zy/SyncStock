using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using SyncStock.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SyncStock
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var login = new SyncStock.Views.LoginForm())
            {
                // 2. Show it. This line waits here until FinishLogin calls this.Close()
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // 3. ONLY NOW we run the MainForm
                    Application.Run(new MainForm(login.LoggedInUser));
                }
            }
            // If DialogResult wasn't OK, the code reaches here and the app closes safely.
        }

        //Test
    }
}
