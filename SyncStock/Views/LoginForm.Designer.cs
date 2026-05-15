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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.PnlCard = new DevExpress.XtraEditors.PanelControl();
            this.BtnEmailTab = new DevExpress.XtraEditors.SimpleButton();
            this.BtnRfidTab = new DevExpress.XtraEditors.SimpleButton();
            this.emailLoginUC = new SyncStock.Views.EmailLoginUC();
            this.rfidLoginUC = new SyncStock.Views.RfidLoginUC();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).BeginInit();
            this.PnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(310, 15);
            this.pictureEdit1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.ZoomPercent = 250D;
            this.pictureEdit1.Size = new System.Drawing.Size(117, 118);
            this.pictureEdit1.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(262, 140);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(236, 46);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Welcome Back!";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(310, 193);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(141, 21);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Sign in to SyncStock";
            // 
            // PnlCard
            // 
            this.PnlCard.Controls.Add(this.emailLoginUC);
            this.PnlCard.Controls.Add(this.rfidLoginUC);
            this.PnlCard.Controls.Add(this.BtnEmailTab);
            this.PnlCard.Controls.Add(this.BtnRfidTab);
            this.PnlCard.Location = new System.Drawing.Point(159, 236);
            this.PnlCard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PnlCard.Name = "PnlCard";
            this.PnlCard.Size = new System.Drawing.Size(455, 380);
            this.PnlCard.TabIndex = 3;
            // 
            // BtnEmailTab
            // 
            this.BtnEmailTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnEmailTab.ImageOptions.SvgImage")));
            this.BtnEmailTab.Location = new System.Drawing.Point(240, 6);
            this.BtnEmailTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnEmailTab.Name = "BtnEmailTab";
            this.BtnEmailTab.Size = new System.Drawing.Size(209, 41);
            this.BtnEmailTab.TabIndex = 1;
            this.BtnEmailTab.Text = "Email & Password";
            this.BtnEmailTab.Click += new System.EventHandler(this.BtnEmailTab_Click);
            // 
            // BtnRfidTab
            // 
            this.BtnRfidTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnRfidTab.ImageOptions.SvgImage")));
            this.BtnRfidTab.Location = new System.Drawing.Point(6, 6);
            this.BtnRfidTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnRfidTab.Name = "BtnRfidTab";
            this.BtnRfidTab.Size = new System.Drawing.Size(209, 41);
            this.BtnRfidTab.TabIndex = 0;
            this.BtnRfidTab.Text = "RFID";
            this.BtnRfidTab.Click += new System.EventHandler(this.BtnRfidTab_Click);
            // 
            // emailLoginUC
            // 
            this.emailLoginUC.Location = new System.Drawing.Point(6, 55);
            this.emailLoginUC.Margin = new System.Windows.Forms.Padding(4);
            this.emailLoginUC.Name = "emailLoginUC";
            this.emailLoginUC.Size = new System.Drawing.Size(442, 319);
            this.emailLoginUC.TabIndex = 3;
            this.emailLoginUC.Visible = false;
            // 
            // rfidLoginUC
            // 
            this.rfidLoginUC.Location = new System.Drawing.Point(7, 55);
            this.rfidLoginUC.Margin = new System.Windows.Forms.Padding(4);
            this.rfidLoginUC.Name = "rfidLoginUC";
            this.rfidLoginUC.Size = new System.Drawing.Size(442, 319);
            this.rfidLoginUC.TabIndex = 2;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 683);
            this.Controls.Add(this.PnlCard);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.pictureEdit1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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