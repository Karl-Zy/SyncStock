using DevExpress.LookAndFeel;
using SyncStock.Views.Theme.ThemeType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Views.Theme
{
    public static class ThemeManager
    {
        public static ThemeMode CurrentMode
        {
            get;
            private set;
        }

        public static void SetTheme(ThemeMode mode)
        {
            CurrentMode = mode;

            string skinName;

            switch(mode)
            {
                case ThemeMode.Dark:
                    skinName = new DarkMode().SkinName;
                    break;
                
                default:
                    skinName = new LightMode().SkinName;
                    break;
            }

            UserLookAndFeel.Default.SetSkinStyle(skinName);
        }
    }
}