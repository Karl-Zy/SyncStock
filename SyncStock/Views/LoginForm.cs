using DevExpress.Utils;
using DevExpress.XtraEditors;
using PCSC;
using PCSC.Exceptions;
using PCSC.Monitoring;
using PCSC.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncStock.Views
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void BtnSimulate_Click(object sender, EventArgs e)
        {
            XtraForm scanDialog = new XtraForm();
            scanDialog.Text = "Ready to Scan";
            scanDialog.Size = new Size(300, 170);
            scanDialog.StartPosition = FormStartPosition.CenterParent;
            scanDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            scanDialog.ControlBox = false;

            LabelControl lbl = new LabelControl();
            lbl.Text = "Please scan your card now...";
            lbl.Appearance.Font = new Font("Segoe UI", 11f);
            lbl.AutoSizeMode = LabelAutoSizeMode.None;
            lbl.Size = new Size(280, 30);
            lbl.Location = new Point(10, 30);
            lbl.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            scanDialog.Controls.Add(lbl);

            // Countdown label
            LabelControl lblCountdown = new LabelControl();
            lblCountdown.Text = "Time remaining: 10s";
            lblCountdown.Appearance.Font = new Font("Segoe UI", 9f);
            lblCountdown.Appearance.ForeColor = Color.Gray;
            lblCountdown.AutoSizeMode = LabelAutoSizeMode.None;
            lblCountdown.Size = new Size(280, 25);
            lblCountdown.Location = new Point(10, 70);
            lblCountdown.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            scanDialog.Controls.Add(lblCountdown);

            // Countdown timer
            int secondsLeft = 10;
            System.Windows.Forms.Timer countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += (ts, te) =>
            {
                secondsLeft--;
                lblCountdown.Text = "Time remaining: " + secondsLeft + "s";

                if (secondsLeft <= 0)
                {
                    countdownTimer.Stop();
                }
            };
            countdownTimer.Start();

            CancellationTokenSource cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                string uid = null;
                DateTime deadline = DateTime.Now.AddSeconds(10);

                // Keep trying until card is read or time runs out
                while (DateTime.Now < deadline)
                {
                    uid = ReadCardUID();
                    if (!string.IsNullOrEmpty(uid)) break;
                    System.Threading.Thread.Sleep(300);
                }

                this.Invoke((MethodInvoker)(() =>
                {
                    countdownTimer.Stop();
                    scanDialog.Close();

                    if (!string.IsNullOrEmpty(uid))
                    {
                        XtraMessageBox.Show(
                            "Thank you! Card scanned successfully.\n\nUID: " + uid,
                            "Card Detected",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        XtraMessageBox.Show(
                            "No card detected. Please try again.",
                            "Scan Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }
                }));
            });

            scanDialog.ShowDialog(this);
            countdownTimer.Stop();
        }

        private string ReadCardUID()
        {
            try
            {
                using (var context = ContextFactory.Instance.Establish(SCardScope.System))
                {
                    var readers = context.GetReaders();
                    if (readers == null || readers.Length == 0) return null;

                    string readerName = readers.FirstOrDefault(r =>
                        r.IndexOf("ACR122", StringComparison.OrdinalIgnoreCase) >= 0)
                        ?? readers[0];

                    using (var reader = new SCardReader(context))
                    {
                        SCardError err = reader.Connect(
                            readerName,
                            SCardShareMode.Shared,
                            SCardProtocol.Any
                        );

                        if (err != SCardError.Success) return null;

                        SCardProtocol activeProtocol = reader.ActiveProtocol;

                        byte[] sendBuffer = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
                        byte[] recvBuffer = new byte[256];

                        err = reader.Transmit(
    SCardPCI.GetPci(activeProtocol),  
    sendBuffer,                        
    null,                              
    ref recvBuffer                     
);

                        if (err != SCardError.Success) return null;
                        if (recvBuffer.Length < 3) return null;

                        byte sw1 = recvBuffer[recvBuffer.Length - 2];
                        byte sw2 = recvBuffer[recvBuffer.Length - 1];
                        if (sw1 != 0x90 || sw2 != 0x00) return null;

                        byte[] uidBytes = recvBuffer.Take(recvBuffer.Length - 2).ToArray();
                        return BitConverter.ToString(uidBytes).Replace("-", ":");
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Debug Error");
                return null;
            }
        }
    }
}