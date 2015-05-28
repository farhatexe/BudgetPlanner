using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<ApplicationUser> HouseholdUsers { get; set; }
        public IEnumerable<BudgetAccount> Accounts { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<BudgetItem> Budgets { get; set; }
        public IList<Category> Categories { get; set; }
        public IList<BudgetItem> BudgetCatAmt { get; set; }
        public IList<Transaction> ActualCatAmt { get; set; }
    }
}