using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncStock.Models.Accounts
{
    public  class AssetManager : User
    {
        public string AccountType { get; set; } = "Asset Manager";
        public AssetManager(string userName, string password, string firstName, string lastName, string accountType) : base(userName, password, firstName, lastName, "Asset")
        {
            AccountType = accountType;
        }
        
        public override void DisplayInfo()
        {
            XtraMessageBox.Show($"Welcome, {FirstName} {LastName}! You are logged in as an {AccountType}.");
        }

    }
}
