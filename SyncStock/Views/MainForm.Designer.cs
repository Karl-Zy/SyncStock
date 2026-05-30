namespace SyncStock
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.dashBoard = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.purchaseOrder = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.reveivingCustodian = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.auditorReview = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.reports = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.mainPanel = new DevExpress.XtraEditors.PanelControl();
            this.sidePanel = new DevExpress.XtraEditors.PanelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.lightMode = new DevExpress.XtraEditors.LabelControl();
            this.darkMode = new DevExpress.XtraEditors.LabelControl();
            this.auditorReviewUC3 = new SyncStock.Views.UserControl.AuditorReviewUC();
            this.dashBoardUC1 = new SyncStock.Views.UserControl.DashBoardUC();
            this.auditorReviewUC2 = new SyncStock.Views.UserControl.AuditorReviewUC();
            this.auditorReviewUC1 = new SyncStock.Views.UserControl.AuditorReviewUC();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sidePanel)).BeginInit();
            this.sidePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // accordionControl1
            // 
            this.accordionControl1.Appearance.AccordionControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(233)))));
            this.accordionControl1.Appearance.AccordionControl.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accordionControl1.Appearance.AccordionControl.Options.UseBackColor = true;
            this.accordionControl1.Appearance.AccordionControl.Options.UseFont = true;
            this.accordionControl1.Appearance.Item.Default.Font = new System.Drawing.Font("Tahoma", 10F);
            this.accordionControl1.Appearance.Item.Default.Options.UseFont = true;
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.dashBoard,
            this.purchaseOrder,
            this.reveivingCustodian,
            this.auditorReview,
            this.reports});
            this.accordionControl1.Location = new System.Drawing.Point(0, 0);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.Size = new System.Drawing.Size(193, 574);
            this.accordionControl1.TabIndex = 0;
            this.accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // dashBoard
            // 
            this.dashBoard.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("dashBoard.ImageOptions.SvgImage")));
            this.dashBoard.Name = "dashBoard";
            this.dashBoard.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.dashBoard.Text = "Dashboard";
            this.dashBoard.Click += new System.EventHandler(this.dashBoard_Click);
            // 
            // purchaseOrder
            // 
            this.purchaseOrder.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("purchaseOrder.ImageOptions.SvgImage")));
            this.purchaseOrder.Name = "purchaseOrder";
            this.purchaseOrder.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.purchaseOrder.Text = "Purchase Order";
            this.purchaseOrder.Click += new System.EventHandler(this.purchaseOrder_Click);
            // 
            // reveivingCustodian
            // 
            this.reveivingCustodian.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("reveivingCustodian.ImageOptions.SvgImage")));
            this.reveivingCustodian.Name = "reveivingCustodian";
            this.reveivingCustodian.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.reveivingCustodian.Text = "Receiving Custodian";
            this.reveivingCustodian.Click += new System.EventHandler(this.reveivingCustodian_Click);
            // 
            // auditorReview
            // 
            this.auditorReview.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("auditorReview.ImageOptions.SvgImage")));
            this.auditorReview.Name = "auditorReview";
            this.auditorReview.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.auditorReview.Text = "Auditor Review";
            this.auditorReview.Click += new System.EventHandler(this.auditorReview_Click);
            // 
            // reports
            // 
            this.reports.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("reports.ImageOptions.SvgImage")));
            this.reports.Name = "reports";
            this.reports.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.reports.Text = "Reports";
            this.reports.Click += new System.EventHandler(this.reports_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.AllowTouchScroll = true;
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.FireScrollEventOnMouseWheel = true;
            this.mainPanel.Location = new System.Drawing.Point(193, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(920, 574);
            this.mainPanel.TabIndex = 1;
            // 
            // sidePanel
            // 
            this.sidePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.sidePanel.Controls.Add(this.panelControl1);
            this.sidePanel.Controls.Add(this.accordionControl1);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 0);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Size = new System.Drawing.Size(193, 574);
            this.sidePanel.TabIndex = 0;
            // 
            // panelControl1
            // 
            this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(233)))));
            this.panelControl1.Appearance.Options.UseBackColor = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.simpleButton1);
            this.panelControl1.Controls.Add(this.lightMode);
            this.panelControl1.Controls.Add(this.darkMode);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 488);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(193, 86);
            this.panelControl1.TabIndex = 1;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(233)))));
            this.simpleButton1.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton1.Appearance.ForeColor = System.Drawing.Color.DarkGreen;
            this.simpleButton1.Appearance.Options.UseBackColor = true;
            this.simpleButton1.Appearance.Options.UseFont = true;
            this.simpleButton1.Appearance.Options.UseForeColor = true;
            this.simpleButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.simpleButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.simpleButton1.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.simpleButton1.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("simpleButton1.ImageOptions.SvgImage")));
            this.simpleButton1.Location = new System.Drawing.Point(0, 55);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(193, 31);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Logout";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // lightMode
            // 
            this.lightMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lightMode.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("lightMode.ImageOptions.SvgImage")));
            this.lightMode.Location = new System.Drawing.Point(73, 17);
            this.lightMode.Name = "lightMode";
            this.lightMode.Size = new System.Drawing.Size(32, 32);
            this.lightMode.TabIndex = 2;
            this.lightMode.Click += new System.EventHandler(this.lightMode_Click);
            // 
            // darkMode
            // 
            this.darkMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.darkMode.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("darkMode.ImageOptions.SvgImage")));
            this.darkMode.Location = new System.Drawing.Point(73, 17);
            this.darkMode.Name = "darkMode";
            this.darkMode.Size = new System.Drawing.Size(32, 32);
            this.darkMode.TabIndex = 1;
            this.darkMode.Click += new System.EventHandler(this.darkMode_Click);
            // 
            // auditorReviewUC3
            // 
            this.auditorReviewUC3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.auditorReviewUC3.Location = new System.Drawing.Point(2, 2);
            this.auditorReviewUC3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.auditorReviewUC3.Name = "auditorReviewUC3";
            this.auditorReviewUC3.Size = new System.Drawing.Size(1570, 862);
            this.auditorReviewUC3.TabIndex = 8;
            // 
            // dashBoardUC1
            // 
            this.dashBoardUC1.AutoScroll = true;
            this.dashBoardUC1.Location = new System.Drawing.Point(16, 16);
            this.dashBoardUC1.Margin = new System.Windows.Forms.Padding(4);
            this.dashBoardUC1.Name = "dashBoardUC1";
            this.dashBoardUC1.Size = new System.Drawing.Size(855, 428);
            this.dashBoardUC1.TabIndex = 4;
            // 
            // auditorReviewUC2
            // 
            this.auditorReviewUC2.Location = new System.Drawing.Point(8, 8);
            this.auditorReviewUC2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.auditorReviewUC2.Name = "auditorReviewUC2";
            this.auditorReviewUC2.Size = new System.Drawing.Size(952, 427);
            this.auditorReviewUC2.TabIndex = 3;
            // 
            // auditorReviewUC1
            // 
            this.auditorReviewUC1.Location = new System.Drawing.Point(0, 0);
            this.auditorReviewUC1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.auditorReviewUC1.Name = "auditorReviewUC1";
            this.auditorReviewUC1.Size = new System.Drawing.Size(1283, 864);
            this.auditorReviewUC1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 574);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.ColorizeInactiveIcon = DevExpress.Utils.DefaultBoolean.True;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("MainForm.IconOptions.SvgImage")));
            this.InactiveGlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SyncStock";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sidePanel)).EndInit();
            this.sidePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement dashBoard;
        private DevExpress.XtraBars.Navigation.AccordionControlElement purchaseOrder;
        private DevExpress.XtraBars.Navigation.AccordionControlElement reveivingCustodian;
        private DevExpress.XtraBars.Navigation.AccordionControlElement auditorReview;
        private DevExpress.XtraBars.Navigation.AccordionControlElement reports;
        private DevExpress.XtraEditors.PanelControl mainPanel;
        private Views.UserControl.AuditorReviewUC auditorReviewUC1;
        private Views.UserControl.AuditorReviewUC auditorReviewUC3;
        private Views.UserControl.PurchaseOrderUC purchaseOrderUC1;
        private Views.UserControl.DashBoardUC dashBoardUC1;
        private Views.UserControl.AuditorReviewUC auditorReviewUC2;
        private DevExpress.XtraEditors.PanelControl sidePanel;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lightMode;
        private DevExpress.XtraEditors.LabelControl darkMode;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}