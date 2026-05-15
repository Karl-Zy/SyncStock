using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SyncStock.Models.Accounts
{
    public class User   
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public string RfidUID { get; set; }

        private string _password;

        public User(){}
        
        public User(string userName, string password, string firstName, string lastName)
        {
            UserName = userName;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
        }
        public string Password
        {
            get { return _password; }
            set
            {
                // We only check if it is null or empty. 
                // We don't tell the user how long it needs to be.
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Invalid input.");
                }
                _password = value;
            }
        }

        public User(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("Username cannot be empty.");
            }

            UserName = userName;
            Password = password;
        }

        public virtual void DisplayInfo()
        {
            XtraMessageBox.Show($"Welcome!");
        }


    }
}
