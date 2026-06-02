using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using SyncStock.Views;
using System;
using System.Windows.Forms;

namespace SyncStock
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            BonusSkins.Register();

            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            bool logout;

            do
            {
                logout = false;

                using (LoginForm login = new LoginForm())
                {
                    if (login.ShowDialog() == DialogResult.OK)
                    {
                        MainForm mainForm =
                            new MainForm(login.LoggedInUser);

                        Application.Run(mainForm);

                        logout = mainForm.LogoutRequested;
                    }
                    else
                    {
                        break;
                    }
                }

            } while (logout);
        }
    }
}