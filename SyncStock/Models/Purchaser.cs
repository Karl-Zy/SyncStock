namespace SyncStock.Models
{
    public class Purchaser : Users
    {
        public Purchaser(string username, string passwordHash, string department)
            : base(username, passwordHash, department)
        {
            Role = UserRole.Purchaser;
        }

        public override bool CanAccessFeature(string featureName)
        {
            return featureName == "CreatePurchase" || featureName == "AttachInvoice";
        }

        // Logic: Items > 5000 are automated as 'Yes' for capitalizable assets
        public bool IsCapitalizable(decimal amount) => amount >= 5000;
    }
}