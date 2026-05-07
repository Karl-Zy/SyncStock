namespace SyncStock.Views
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.PnlCard = new DevExpress.XtraEditors.PanelControl();
            this.BtnRfidTab = new DevExpress.XtraEditors.SimpleButton();
            this.BtnEmailTab = new DevExpress.XtraEditors.SimpleButton();
            this.PnlRfid = new DevExpress.XtraEditors.PanelControl();
            this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.BtnSimulate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).BeginInit();
            this.PnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PnlRfid)).BeginInit();
            this.PnlRfid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(266, 12);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.ZoomAcceleration = 5D;
            this.pictureEdit1.Properties.ZoomPercent = 250D;
            this.pictureEdit1.Size = new System.Drawing.Size(100, 96);
            this.pictureEdit1.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(225, 114);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(186, 37);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Welcome Back!";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(266, 157);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(117, 17);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Sign in to SyncStock";
            // 
            // PnlCard
            // 
            this.PnlCard.Controls.Add(this.PnlRfid);
            this.PnlCard.Controls.Add(this.BtnEmailTab);
            this.PnlCard.Controls.Add(this.BtnRfidTab);
            this.PnlCard.Location = new System.Drawing.Point(136, 192);
            this.PnlCard.Name = "PnlCard";
            this.PnlCard.Size = new System.Drawing.Size(390, 309);
            this.PnlCard.TabIndex = 2;
            // 
            // BtnRfidTab
            // 
            this.BtnRfidTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnRfidTab.ImageOptions.SvgImage")));
            this.BtnRfidTab.Location = new System.Drawing.Point(5, 5);
            this.BtnRfidTab.Name = "BtnRfidTab";
            this.BtnRfidTab.Size = new System.Drawing.Size(179, 33);
            this.BtnRfidTab.TabIndex = 0;
            this.BtnRfidTab.Text = "RFID";
            // 
            // BtnEmailTab
            // 
            this.BtnEmailTab.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("BtnEmailTab.ImageOptions.SvgImage")));
            this.BtnEmailTab.Location = new System.Drawing.Point(206, 5);
            this.BtnEmailTab.Name = "BtnEmailTab";
            this.BtnEmailTab.Size = new System.Drawing.Size(179, 33);
            this.BtnEmailTab.TabIndex = 0;
            this.BtnEmailTab.Text = "Email & Password";
            // 
            // PnlRfid
            // 
            this.PnlRfid.Controls.Add(this.BtnSimulate);
            this.PnlRfid.Controls.Add(this.labelControl4);
            this.PnlRfid.Controls.Add(this.labelControl3);
            this.PnlRfid.Controls.Add(this.pictureEdit2);
            this.PnlRfid.Location = new System.Drawing.Point(6, 45);
            this.PnlRfid.Name = "PnlRfid";
            this.PnlRfid.Size = new System.Drawing.Size(379, 259);
            this.PnlRfid.TabIndex = 1;
            // 
            // pictureEdit2
            // 
            this.pictureEdit2.EditValue = ((object)(resources.GetObject("pictureEdit2.EditValue")));
            this.pictureEdit2.Location = new System.Drawing.Point(124, 5);
            this.pictureEdit2.Name = "pictureEdit2";
            this.pictureEdit2.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit2.Size = new System.Drawing.Size(100, 77);
            this.pictureEdit2.TabIndex = 0;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(124, 88);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(110, 23);
            this.labelControl3.TabIndex = 1;
            this.labelControl3.Text = "Ready to Scan";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(142, 117);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(82, 17);
            this.labelControl4.TabIndex = 1;
            this.labelControl4.Text = "Tap  ID  Card ";
            // 
            // BtnSimulate
            // 
            this.BtnSimulate.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSimulate.Appearance.Options.UseFont = true;
            this.BtnSimulate.Location = new System.Drawing.Point(94, 153);
            this.BtnSimulate.Name = "BtnSimulate";
            this.BtnSimulate.Size = new System.Drawing.Size(175, 53);
            this.BtnSimulate.TabIndex = 2;
            this.BtnSimulate.Text = "Simulate Card Scan";
            this.BtnSimulate.Click += new System.EventHandler(this.BtnSimulate_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 555);
            this.Controls.Add(this.PnlCard);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.pictureEdit1);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PnlCard)).EndInit();
            this.PnlCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PnlRfid)).EndInit();
            this.PnlRfid.ResumeLayout(false);
            this.PnlRfid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit2.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl PnlCard;
        private DevExpress.XtraEditors.PanelControl PnlRfid;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.PictureEdit pictureEdit2;
        private DevExpress.XtraEditors.SimpleButton BtnEmailTab;
        private DevExpress.XtraEditors.SimpleButton BtnRfidTab;
        private DevExpress.XtraEditors.SimpleButton BtnSimulate;
        private DevExpress.XtraEditors.LabelControl labelControl4;
    }
}