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
                // We validate that it's not null, but we don't tell the user 
                // WHY a password might be rejected during the assignment.
                if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
                {
                    throw new ArgumentException("Invalid entry.");
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
