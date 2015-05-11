using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class DashboardViewModel
    {
        public string HouseholdName { get; set; }
        public string HouseholdUsers { get; set; }
        public string AccountName { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal BudgetAmount { get; set; }
        public string BudgetIncExp { get; set; }
        public decimal CatTransAmount { get; set; }
        public decimal NotReconciledAmount { get; set; }

        //public DashboadViewModel(Transaction transaction)
        //{

        //}


    }
}