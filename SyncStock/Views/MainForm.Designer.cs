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
            this.auditorReviewUC3 = new SyncStock.Views.UserControl.AuditorReviewUC();
            this.dashBoardUC1 = new SyncStock.Views.UserControl.DashBoardUC();
            this.auditorReviewUC2 = new SyncStock.Views.UserControl.AuditorReviewUC();
            this.auditorReviewUC1 = new SyncStock.Views.UserControl.AuditorReviewUC();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // accordionControl1
            // 
            this.accordionControl1.Appearance.Item.Default.Font = new System.Drawing.Font("Tahoma", 10F);
            this.accordionControl1.Appearance.Item.Default.Options.UseFont = true;
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.dashBoard,
            this.purchaseOrder,
            this.reveivingCustodian,
            this.auditorReview,
            this.reports});
            this.accordionControl1.Location = new System.Drawing.Point(0, 0);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.Size = new System.Drawing.Size(89, 574);
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
            this.mainPanel.AutoSize = true;
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.FireScrollEventOnMouseWheel = true;
            this.mainPanel.Location = new System.Drawing.Point(89, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(4);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1024, 574);
            this.mainPanel.TabIndex = 1;
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
            this.Controls.Add(this.accordionControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.ColorizeInactiveIcon = DevExpress.Utils.DefaultBoolean.True;
            this.IconOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("MainForm.IconOptions.SvgImage")));
            this.InactiveGlowColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SyncStock";
            
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private Views.UserControl.DashBoardUC dashBoardUC1;
        private Views.UserControl.AuditorReviewUC auditorReviewUC2;
    }
}