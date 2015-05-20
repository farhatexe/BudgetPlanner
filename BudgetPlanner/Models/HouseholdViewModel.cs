using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class HouseholdViewModel
    {
        public string Name { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }

    }
}