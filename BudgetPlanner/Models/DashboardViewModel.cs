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

        //public DashboardViewModel(Household household)
        //{
        //    this.HouseholdName = household.Name;
        //    this.HouseholdUsers = household.Users.SelectMany(u => u.HouseholdId = household.Id);
        //    this.AccountName = household.BudgetAccounts.SelectMany(a => a.HouseholdId == household.Id);
        //}
    }
}