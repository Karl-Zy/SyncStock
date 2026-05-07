using System;

namespace SyncStock.Models
{
    public enum PeriodStatus
    {
        Open,
        Locked,
        UnlockPending,   // Auditor requested, awaiting Asset Manager approval
        Unlocked
    }

    public enum PeriodType
    {
        Weekly,
        Monthly
    }

    public class ReconciliationPeriod
    {
        public int Id { get; set; }
        public PeriodType PeriodType { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public PeriodStatus Status { get; set; } = PeriodStatus.Open;

        // Set by Internal Auditor when locking
        public int? LockedByUserId { get; set; }
        public DateTime? LockedAt { get; set; }

        // Set by Internal Auditor when requesting unlock
        public int? UnlockRequestedByUserId { get; set; }
        public DateTime? UnlockRequestedAt { get; set; }

        // Set by Asset Manager when approving unlock
        public int? UnlockedByUserId { get; set; }
        public DateTime? UnlockedAt { get; set; }

        /// <summary>
        /// Checks whether a given transaction date falls inside this locked period.
        /// Used to guard edits on Purchase, PO, and Receiving Report forms.
        /// </summary>
        public bool ContainsDate(DateTime date)
            => date.Date >= PeriodStart.Date && date.Date <= PeriodEnd.Date;

        /// <summary>
        /// Returns true if this period is currently locked or pending unlock,
        /// meaning no edits are allowed.
        /// </summary>
        public bool IsEditBlocked()
            => Status == PeriodStatus.Locked || Status == PeriodStatus.UnlockPending;
    }
}