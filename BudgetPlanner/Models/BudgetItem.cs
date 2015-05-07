using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public int? CategoryId { get; set; }
        public decimal Amount { get; set; }
        public bool Classification { get; set; }              // designates Income(true), Expense(false)

        public virtual Category Category { get; set; }
        public virtual Household Household { get; set; }
    }
}