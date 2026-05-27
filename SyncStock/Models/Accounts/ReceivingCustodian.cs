using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Accounts
{
    internal class ReceivingCustodian : User
    {
        public string AccountType { get; set; } = "Receiving Custodian";
        public ReceivingCustodian(string userName, string password, string firstName, string lastName, string accountType) : base(userName, password, firstName, lastName, "Receiving")
        {
            AccountType = accountType;
        }

        public override void DisplayInfo()
        {
             XtraMessageBox.Show($"Welcome, {FirstName} {LastName}! You are logged in as a {AccountType}.");
        }
    }
}
