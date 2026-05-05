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
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // accordionControl1
            // 
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.dashBoard,
            this.purchaseOrder,
            this.reveivingCustodian,
            this.auditorReview,
            this.reports});
            this.accordionControl1.Location = new System.Drawing.Point(0, 0);
            this.accordionControl1.Margin = new System.Windows.Forms.Padding(4);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.Size = new System.Drawing.Size(302, 1066);
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
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(302, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(4);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1498, 1066);
            this.mainPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1800, 1066);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.accordionControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainPanel)).EndInit();
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
    }
}