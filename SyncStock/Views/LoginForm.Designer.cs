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
            // PnlCard
            // 
            this.PnlCard.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(162)))), ((int)(((byte)(161)))));
            this.PnlCard.Appearance.Options.UseBackColor = true;
            this.PnlCard.Controls.Add(this.emailLoginUC);
            this.PnlCard.Controls.Add(this.panelControl1);
            this.PnlCard.Controls.Add(this.rfidLoginUC);
            this.PnlCard.Location = new System.Drawing.Point(395, 66);
            this.PnlCard.Name = "PnlCard";
            this.PnlCard.Size = new System.Drawing.Size(388, 309);
            this.PnlCard.TabIndex = 3;
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(162)))), ((int)(((byte)(161)))));
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.BtnEmailTab);
            this.panelControl1.Controls.Add(this.BtnRfidTab);
            this.panelControl1.Location = new System.Drawing.Point(-8, -1);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(407, 49);
            this.panelControl1.TabIndex = 5;
            // 
            // BtnEmailTab
            // 
            this.BtnEmailTab.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnEmailTab.Appearance.Options.UseFont = true;
            this.BtnEmailTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnEmailTab.ImageOptions.SvgImage")));
            this.BtnEmailTab.Location = new System.Drawing.Point(203, 6);
            this.BtnEmailTab.Name = "BtnEmailTab";
            this.BtnEmailTab.Size = new System.Drawing.Size(179, 33);
            this.BtnEmailTab.TabIndex = 1;
            this.BtnEmailTab.Text = "Email & Password";
            this.BtnEmailTab.Click += new System.EventHandler(this.BtnEmailTab_Click);
            // 
            // BtnRfidTab
            // 
            this.BtnRfidTab.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRfidTab.Appearance.Options.UseFont = true;
            this.BtnRfidTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnRfidTab.ImageOptions.SvgImage")));
            this.BtnRfidTab.Location = new System.Drawing.Point(18, 6);
            this.BtnRfidTab.Name = "BtnRfidTab";
            this.BtnRfidTab.Size = new System.Drawing.Size(179, 33);
            this.BtnRfidTab.TabIndex = 0;
            this.BtnRfidTab.Text = "RFID";
            this.BtnRfidTab.Click += new System.EventHandler(this.BtnRfidTab_Click);
            // 
            // pictureEdit2
            // 
            this.pictureEdit2.BackgroundImage = global::SyncStock.Properties.Resources.back_dark_UI_copy;
            this.pictureEdit2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureEdit2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEdit2.Location = new System.Drawing.Point(0, 0);
            this.pictureEdit2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureEdit2.Name = "pictureEdit2";
            this.pictureEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit2.Properties.NullText = " ";
            this.pictureEdit2.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit2.Size = new System.Drawing.Size(827, 453);
            this.pictureEdit2.TabIndex = 4;
            this.pictureEdit2.EditValueChanged += new System.EventHandler(this.pictureEdit2_EditValueChanged);
            // 
            // emailLoginUC
            // 
            this.emailLoginUC.Location = new System.Drawing.Point(-8, 44);
            this.emailLoginUC.Name = "emailLoginUC";
            this.emailLoginUC.Size = new System.Drawing.Size(401, 280);
            this.emailLoginUC.TabIndex = 3;
            this.emailLoginUC.Visible = false;
            // 
            // rfidLoginUC
            // 
            this.rfidLoginUC.Location = new System.Drawing.Point(-8, 44);
            this.rfidLoginUC.Name = "rfidLoginUC";
            this.rfidLoginUC.Size = new System.Drawing.Size(407, 265);
            this.rfidLoginUC.TabIndex = 2;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 453);
            this.Controls.Add(this.PnlCard);
            this.Controls.Add(this.pictureEdit2);
            this.MaximizeBox = false;
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

        }
        private DevExpress.XtraEditors.PanelControl PnlCard;
        private DevExpress.XtraEditors.SimpleButton BtnRfidTab;
        private DevExpress.XtraEditors.SimpleButton BtnEmailTab;
        private RfidLoginUC rfidLoginUC;
        private EmailLoginUC emailLoginUC;
        private DevExpress.XtraEditors.PictureEdit pictureEdit2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
    }
}