using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncStock.Views.UserControl
{
    public partial class MeetTheTeam : System.Windows.Forms.UserControl
    {
        public MeetTheTeam()
        {
            InitializeComponent();

            MakeCircular(pictureEdit4);
            MakeCircular(pictureEdit9);
            MakeCircular(pictureEdit5);
            MakeCircular(pictureEdit6);
            MakeCircular(pictureEdit7);
            MakeCircular(pictureEdit8);
        }

        

        private void MakeCircular(PictureEdit picture)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddEllipse(
                0,
                0,
                picture.Width,
                picture.Height);

            picture.Region = new Region(path);
        }
    }
}
