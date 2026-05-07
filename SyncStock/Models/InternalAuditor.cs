using System;
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
            return featureName == "LockMonth"
                || featureName == "ViewReports"
                || featureName == "AddAccounts"
                || featureName == "RequestUnlock";
        }

        /// <summary>
        /// Submits an unlock request for a locked period.
        /// Only the Asset Manager can approve it — the auditor cannot self-approve.
        /// Returns false if the period is not currently locked.
        /// </summary>
        public bool RequestUnlock(ReconciliationPeriod period)
        {
            if (period.Status != PeriodStatus.Locked)
                return false;

            period.UnlockRequestedAt = DateTime.UtcNow;
            period.UnlockRequestedByUserId = this.Id;
            period.Status = PeriodStatus.UnlockPending;
            return true;
        }
    }
}