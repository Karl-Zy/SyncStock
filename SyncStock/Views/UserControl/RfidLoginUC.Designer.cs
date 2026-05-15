namespace SyncStock.Views
{
    partial class RfidLoginUC
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblSubtitle = new DevExpress.XtraEditors.LabelControl();
            this.BtnSimulate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            this.SuspendLayout();

            // pictureEdit1
            this.pictureEdit1.Location = new System.Drawing.Point(124, 5);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Size = new System.Drawing.Size(100, 77);
            this.pictureEdit1.TabIndex = 0;

            // lblTitle
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.Location = new System.Drawing.Point(124, 88);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Ready to Scan";

            // lblSubtitle
            this.lblSubtitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitle.Appearance.Options.UseFont = true;
            this.lblSubtitle.Location = new System.Drawing.Point(142, 117);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Tap ID Card";

            // BtnSimulate
            this.BtnSimulate.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.BtnSimulate.Appearance.Options.UseFont = true;
            this.BtnSimulate.Location = new System.Drawing.Point(94, 153);
            this.BtnSimulate.Name = "BtnSimulate";
            this.BtnSimulate.Size = new System.Drawing.Size(175, 53);
            this.BtnSimulate.TabIndex = 3;
            this.BtnSimulate.Text = "Simulate Card Scan";
            this.BtnSimulate.Click += new System.EventHandler(this.BtnSimulate_Click);

            // RfidLoginUC
            this.Controls.Add(this.BtnSimulate);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pictureEdit1);
            this.Name = "RfidLoginUC";
            this.Size = new System.Drawing.Size(379, 259);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
        }

        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.LabelControl lblSubtitle;
        private DevExpress.XtraEditors.SimpleButton BtnSimulate;
    }
}