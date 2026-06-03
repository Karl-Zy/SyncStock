using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Accounts
{
    internal class InternalAuditor : User
    {
        public string AccountType { get; set; } = "Internal Auditor";
        public InternalAuditor(string userName, string password, string firstName, string lastName, string accountType) : base(userName, password, firstName, lastName, "Auditor")
        {
            AccountType = accountType;
        }   
        public override void DisplayInfo()
        {
            XtraMessageBox.Show($"Welcome, {FirstName} {LastName}! You are logged in as an {AccountType}.");
        }
    }
}
