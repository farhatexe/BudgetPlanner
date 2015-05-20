    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class DashboardViewModel
    {
        public string HouseholdName { get; set; }
        public IEnumerable<ApplicationUser> HouseholdUsers { get; set; }
        public IEnumerable<BudgetAccount> Accounts { get; set; }
        public IEnumerable<BudgetItem> Budgets { get; set; }

    }
}