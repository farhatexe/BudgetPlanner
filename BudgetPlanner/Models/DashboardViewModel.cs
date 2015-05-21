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
        public IEnumerable<BudgetItem> Budgets { get; set; }

        //public IEnumerable<ApplicationUser> HouseholdUsers { get; set; }
        //public IEnumerable<BudgetAccount> Accounts { get; set; }
        //public IEnumerable<BudgetItem> BudgetIncome { get; set; }
        //public IEnumerable<BudgetItem> BudgetExpense { get; set; }

        //public DashboardViewModel(Household household)
        //{
        //    // get sum of all budget item incomes
        //    this.BudgetIncome = household.BudgetItems.Sum(b=> b.Categories.GroupBy(c=> c.Income == true)).
        //}
    }
}