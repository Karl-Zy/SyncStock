using System;
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

        /// <summary>
        /// An item is capitalizable when its total value (quantity × unit amount)
        /// meets or exceeds the ₱5,000 threshold.
        /// </summary>
        public bool IsCapitalizable(int quantity, decimal unitAmount)
            => quantity * unitAmount >= 5000m;

        /// <summary>
        /// A collection of individual items should be grouped into a single
        /// capitalizable asset when their combined total meets the threshold.
        /// </summary>
        public bool ShouldGroupItems(decimal combinedTotal)
            => combinedTotal >= 5000m;
    }
}