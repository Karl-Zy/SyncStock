namespace SyncStock.Models
{
    public class InternalAuditor : Users
    {
        public InternalAuditor(string username, string passwordHash, string department)
            : base(username, passwordHash, department)
        {
            Role = UserRole.InternalAuditor;
        }

        public override bool CanAccessFeature(string featureName)
        {
            // Auditors can lock files and view/add accounts
            return featureName == "LockMonth" || featureName == "ViewReports" || featureName == "AddAccounts";
        }
    }
}