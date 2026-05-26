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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.PnlCard = new DevExpress.XtraEditors.PanelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.BtnEmailTab = new DevExpress.XtraEditors.SimpleButton();
            this.BtnRfidTab = new DevExpress.XtraEditors.SimpleButton();
            this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
            this.emailLoginUC = new SyncStock.Views.EmailLoginUC();
            this.rfidLoginUC = new SyncStock.Views.RfidLoginUC();
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).BeginInit();
            this.PnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(175, 138);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(236, 46);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Welcome Back!";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(219, 191);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(141, 21);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Sign in to SyncStock";
            // 
            // PnlCard
            // 
            this.PnlCard.Controls.Add(this.panelControl1);
            this.PnlCard.Controls.Add(this.emailLoginUC);
            this.PnlCard.Controls.Add(this.rfidLoginUC);
            this.PnlCard.Location = new System.Drawing.Point(59, 234);
            this.PnlCard.Margin = new System.Windows.Forms.Padding(4);
            this.PnlCard.Name = "PnlCard";
            this.PnlCard.Size = new System.Drawing.Size(455, 380);
            this.PnlCard.TabIndex = 3;
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(242)))));
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.BtnEmailTab);
            this.panelControl1.Controls.Add(this.BtnRfidTab);
            this.panelControl1.Location = new System.Drawing.Point(5, 5);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(444, 54);
            this.panelControl1.TabIndex = 5;
            // 
            // BtnEmailTab
            // 
            this.BtnEmailTab.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnEmailTab.Appearance.Options.UseFont = true;
            this.BtnEmailTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnEmailTab.ImageOptions.SvgImage")));
            this.BtnEmailTab.Location = new System.Drawing.Point(224, 7);
            this.BtnEmailTab.Margin = new System.Windows.Forms.Padding(4);
            this.BtnEmailTab.Name = "BtnEmailTab";
            this.BtnEmailTab.Size = new System.Drawing.Size(209, 41);
            this.BtnEmailTab.TabIndex = 1;
            this.BtnEmailTab.Text = "Email & Password";
            this.BtnEmailTab.Click += new System.EventHandler(this.BtnEmailTab_Click);
            // 
            // BtnRfidTab
            // 
            this.BtnRfidTab.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRfidTab.Appearance.Options.UseFont = true;
            this.BtnRfidTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnRfidTab.ImageOptions.SvgImage")));
            this.BtnRfidTab.Location = new System.Drawing.Point(12, 7);
            this.BtnRfidTab.Margin = new System.Windows.Forms.Padding(4);
            this.BtnRfidTab.Name = "BtnRfidTab";
            this.BtnRfidTab.Size = new System.Drawing.Size(209, 41);
            this.BtnRfidTab.TabIndex = 0;
            this.BtnRfidTab.Text = "RFID";
            this.BtnRfidTab.Click += new System.EventHandler(this.BtnRfidTab_Click);
            // 
            // pictureEdit2
            // 
            this.pictureEdit2.BackgroundImage = global::SyncStock.Properties.Resources.logo;
            this.pictureEdit2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureEdit2.Location = new System.Drawing.Point(193, 12);
            this.pictureEdit2.Name = "pictureEdit2";
            this.pictureEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit2.Properties.NullText = " ";
            this.pictureEdit2.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit2.Size = new System.Drawing.Size(218, 147);
            this.pictureEdit2.TabIndex = 4;
            this.pictureEdit2.EditValueChanged += new System.EventHandler(this.pictureEdit2_EditValueChanged);
            // 
            // emailLoginUC
            // 
            this.emailLoginUC.Location = new System.Drawing.Point(1, 59);
            this.emailLoginUC.Margin = new System.Windows.Forms.Padding(4);
            this.emailLoginUC.Name = "emailLoginUC";
            this.emailLoginUC.Size = new System.Drawing.Size(449, 319);
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
            this.ClientSize = new System.Drawing.Size(580, 683);
            this.Controls.Add(this.PnlCard);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.pictureEdit2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SyncStock Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).EndInit();
            this.PnlCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl PnlCard;
        private DevExpress.XtraEditors.SimpleButton BtnRfidTab;
        private DevExpress.XtraEditors.SimpleButton BtnEmailTab;
        private RfidLoginUC rfidLoginUC;
        private EmailLoginUC emailLoginUC;
        private DevExpress.XtraEditors.PictureEdit pictureEdit2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}