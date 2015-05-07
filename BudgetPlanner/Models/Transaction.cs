using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal AbsAmount { get; set; }
        public decimal ReconciledAmount { get; set; }
        public decimal AbsReconciledAmount { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int UpdateByUserId { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual BudgetAccount BudgetAccount { get; set; }
        public virtual ApplicationUser UpdatedByUser { get; set; }
    }
}