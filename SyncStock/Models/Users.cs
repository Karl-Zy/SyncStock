using System;
namespace SyncStock.Models
{
    public enum UserRole
    {
        Purchaser,
        ReceivingCustodian,
        AssetManager,
        InternalAuditor
    }

    public abstract class Users
    {
        // Encapsulation: Properties manage access to data fields
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string DepartmentID { get; set; } = string.Empty;

        // Protected set ensures the role is "locked" upon instantiation
        public UserRole Role { get; protected set; }

        protected Users(string username, string passwordHash, string department)
        {
            Username = username;
            PasswordHash = passwordHash;
            DepartmentID = department;
        }

        // Polymorphism: Abstract method for role-specific security checks
        public abstract bool CanAccessFeature(string featureName);
    }
}