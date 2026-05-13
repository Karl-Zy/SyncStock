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

            // 1. Initialize the Login Form
            using (var login = new SyncStock.Views.LoginForm())
            {
                // 2. ShowDialog blocks execution until the form is closed
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // 3. Hand over the LoggedInUser object to the MainForm
                    Application.Run(new MainForm(login.LoggedInUser));
                }
                // If they click 'X' or login fails, the app ends here safely.
            }
        }

        //Test
    }
}
