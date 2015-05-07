using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public string ToEmail { get; set; }
        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }
        public virtual ApplicationUser FromUser { get; set; }
    }
}