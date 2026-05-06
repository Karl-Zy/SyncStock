namespace SyncStock.Models
{
    public class ReceivingCustodian : Users
    {
        public ReceivingCustodian(string username, string passwordHash, string department)
            : base(username, passwordHash, department)
        {
            Role = UserRole.ReceivingCustodian;
        }

        public override bool CanAccessFeature(string featureName)
        {
            // Focuses on receiving reports and adding remarks
            return featureName == "ReceiveReport" || featureName == "AddRemarks";
        }
    }
}