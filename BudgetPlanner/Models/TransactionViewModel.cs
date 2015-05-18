using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class TransactionViewModel
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public bool Reconciled { get; set; }
        public string UpdateBy { get; set; }

        public TransactionViewModel(Transaction transaction)
        {
            this.Date = transaction.Date.ToString("d");
            this.Description = "<a href=\"/Transaction/Edit/" + transaction.Id + "\">" + transaction.Description + "</a>";
            this.Amount = transaction.Amount;
            this.Category = transaction.Category.Name;
            this.Reconciled = transaction.Reconciled;
            this.UpdateBy = transaction.UpdateByUser.Name;

        }
    }

    
}