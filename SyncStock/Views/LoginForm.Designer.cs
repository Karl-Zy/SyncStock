namespace SyncStock.Views
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));

            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.PnlCard = new DevExpress.XtraEditors.PanelControl();
            this.BtnRfidTab = new DevExpress.XtraEditors.SimpleButton();
            this.BtnEmailTab = new DevExpress.XtraEditors.SimpleButton();
            this.rfidLoginUC = new RfidLoginUC();
            this.emailLoginUC = new EmailLoginUC();

            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).BeginInit();
            this.PnlCard.SuspendLayout();
            this.SuspendLayout();

            // pictureEdit1
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(266, 12);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.ZoomPercent = 250D;
            this.pictureEdit1.Size = new System.Drawing.Size(100, 96);
            this.pictureEdit1.TabIndex = 0;

            // labelControl1
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(225, 114);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Welcome Back!";

            // labelControl2
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(266, 157);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Sign in to SyncStock";

            // BtnRfidTab
            this.BtnRfidTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnRfidTab.ImageOptions.SvgImage")));
            this.BtnRfidTab.Location = new System.Drawing.Point(5, 5);
            this.BtnRfidTab.Name = "BtnRfidTab";
            this.BtnRfidTab.Size = new System.Drawing.Size(179, 33);
            this.BtnRfidTab.TabIndex = 0;
            this.BtnRfidTab.Text = "RFID";
            this.BtnRfidTab.Click += new System.EventHandler(this.BtnRfidTab_Click);

            // BtnEmailTab
            this.BtnEmailTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnEmailTab.ImageOptions.SvgImage")));
            this.BtnEmailTab.Location = new System.Drawing.Point(206, 5);
            this.BtnEmailTab.Name = "BtnEmailTab";
            this.BtnEmailTab.Size = new System.Drawing.Size(179, 33);
            this.BtnEmailTab.TabIndex = 1;
            this.BtnEmailTab.Text = "Email & Password";
            this.BtnEmailTab.Click += new System.EventHandler(this.BtnEmailTab_Click);

            // rfidLoginUC
            this.rfidLoginUC.Location = new System.Drawing.Point(6, 45);
            this.rfidLoginUC.Name = "rfidLoginUC";
            this.rfidLoginUC.Size = new System.Drawing.Size(379, 259);
            this.rfidLoginUC.TabIndex = 2;

            // emailLoginUC
            this.emailLoginUC.Location = new System.Drawing.Point(6, 45);
            this.emailLoginUC.Name = "emailLoginUC";
            this.emailLoginUC.Size = new System.Drawing.Size(379, 259);
            this.emailLoginUC.TabIndex = 3;
            this.emailLoginUC.Visible = false;

            // PnlCard
            this.PnlCard.Controls.Add(this.emailLoginUC);
            this.PnlCard.Controls.Add(this.rfidLoginUC);
            this.PnlCard.Controls.Add(this.BtnEmailTab);
            this.PnlCard.Controls.Add(this.BtnRfidTab);
            this.PnlCard.Location = new System.Drawing.Point(136, 192);
            this.PnlCard.Name = "PnlCard";
            this.PnlCard.Size = new System.Drawing.Size(390, 309);
            this.PnlCard.TabIndex = 3;

            // LoginForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 555);
            this.Controls.Add(this.PnlCard);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.pictureEdit1);
            this.Name = "LoginForm";
            this.Text = "SyncStock Login";

            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).EndInit();
            this.PnlCard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl PnlCard;
        private DevExpress.XtraEditors.SimpleButton BtnRfidTab;
        private DevExpress.XtraEditors.SimpleButton BtnEmailTab;
        private RfidLoginUC rfidLoginUC;
        private EmailLoginUC emailLoginUC;
    }
}