using DevExpress.XtraEditors;
using System;

namespace SyncStock.Models.Accounts
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public string RfidUID { get; set; }

        private string _password;

        public User() { }

        // MAIN CONSTRUCTOR
        public User(
            string userName,
            string password,
            string firstName,
            string lastName,
            string role)
        {
            UserName = userName;

            Password = password;

            FirstName = firstName;

            LastName = lastName;

            Role = role;
        }

        public string Password
        {
            get { return _password; }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Invalid input.");
                }

                _password = value;
            }
        }

        // LOGIN CONSTRUCTOR
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