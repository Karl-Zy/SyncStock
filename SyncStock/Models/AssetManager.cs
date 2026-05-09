using System;
namespace SyncStock.Models
{
    public class AssetManager : Users
    {
        public AssetManager(string username, string passwordHash, string department)
            : base(username, passwordHash, department)
        {
            Role = UserRole.AssetManager;
        }

        public override bool CanAccessFeature(string featureName)
        {
            // Asset Manager has master access
            return true;
        }

        /// <summary>
        /// Approves a pending unlock request raised by an Internal Auditor.
        /// Only succeeds when the period is in UnlockPending state,
        /// ensuring the two-step workflow is respected.
        /// </summary>
        public bool ApproveUnlock(ReconciliationPeriod period)
        {
            if (period.Status != PeriodStatus.UnlockPending)
                return false;

            period.Status = PeriodStatus.Unlocked;
            period.UnlockedAt = DateTime.UtcNow;
            period.UnlockedByUserId = this.Id;
            return true;
        }
    }
}