using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BudgetPlanner.Models
{
    public class TransactionViewModel
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Reconciled { get; set; }
        public string UpdateBy { get; set; }
       // public string Delete { get; set; }

        public TransactionViewModel()
        {


        }
    }

    
}