using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Accounts
{
    internal class Purchaser : User
    {
        public string AccountType { get; set; } = "Purchaser";
        public Purchaser(string userName, string password, string firstName, string lastName, string accountType) : base(userName, password, firstName, lastName, "Purchaser")
        {
            AccountType = accountType;
        }
        public override void DisplayInfo()
        {
            XtraMessageBox.Show($"Welcome, {FirstName} {LastName}! You are logged in as a {AccountType}.");
        }
    }
}
