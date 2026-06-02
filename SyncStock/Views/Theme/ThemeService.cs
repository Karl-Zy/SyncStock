using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Views.Theme
{
    public class ThemeService
    {
        public static void SaveTheme (ThemeMode mode)
        {
            Properties.Settings.Default.ThemeMode = mode.ToString();
            Properties.Settings.Default.Save();
        }

        public static ThemeMode LoadTheme()
        {
            string savedTheme = Properties.Settings.Default.ThemeMode;

            if (Enum.TryParse(savedTheme, out ThemeMode mode))
            {
                return mode;
            }
            else
            {
                return ThemeMode.Light; // Default to Light if parsing fails
            }
        }
    }
}
