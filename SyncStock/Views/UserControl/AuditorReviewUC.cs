using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
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
    public partial class AuditorReviewUC : DevExpress.XtraEditors.XtraUserControl
    {
        public AuditorReviewUC()
        {
            InitializeComponent();
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            // --- STATUS column ---
            if (e.Column.FieldName == "Status")
            {
                string val = e.CellValue?.ToString();

                Color bgColor, textColor;

                if (val == "Active")
                {
                    bgColor = Color.FromArgb(220, 247, 220); // light green
                    textColor = Color.FromArgb(30, 120, 30);
                }
                else if (val == "Pending")
                {
                    bgColor = Color.FromArgb(255, 243, 200); // light yellow
                    textColor = Color.FromArgb(160, 100, 0);
                }
                else
                {
                    return; // let DevExpress draw it normally
                }

                DrawBadge(e, val, bgColor, textColor);
                e.Handled = true; // skip default drawing
            }

            // --- CAPITALIZABLE column ---
            if (e.Column.FieldName == "Capitalizable")
            {
                string val = e.CellValue?.ToString();

                if (val == "Yes")
                {
                    DrawBadge(e, val,
                        Color.FromArgb(220, 235, 255), // light blue bg
                        Color.FromArgb(30, 80, 180));  // dark blue text
                    e.Handled = true;
                }
                // "No" — leave as plain text, don't set e.Handled
            }
        }
        private void DrawBadge(RowCellCustomDrawEventArgs e,
                       string text,
                       Color bgColor,
                       Color textColor)
        {
            Graphics g = e.Graphics;

            // 1. Fill the cell background first (matches grid row bg)
            e.DefaultDraw(); // draw selection highlight etc.

            // 2. Define badge rectangle — centered & padded inside the cell
            int padX = 8, padY = 4;
            Rectangle cell = e.Bounds;
            Rectangle badge = new Rectangle(
                cell.X + padX,
                cell.Y + padY,
                cell.Width - (padX * 2),
                cell.Height - (padY * 2)
            );

            // 3. Draw rounded rectangle background
            using (GraphicsPath path = GetRoundedRect(badge, 10))
            using (SolidBrush brush = new SolidBrush(bgColor))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(brush, path);
            }

            // 4. Draw the text centered inside badge
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (Font font = new Font("Segoe UI", 8f, FontStyle.Regular))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, textBrush, badge, sf);
            }
        }
        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.X + rect.Width - d, rect.Y + rect.Height - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
