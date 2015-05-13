using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public decimal? ReconciledAmount { get; set; }
        public decimal AbsReconciledAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Updated { get; set; }
        public string UpdateByUserId { get; set; }
        public int? CategoryId { get; set; }
        public bool Reconciled { get; set; }

        public virtual Category Category { get; set; }
        public virtual ApplicationUser UpdateByUser { get; set; }
    }
}