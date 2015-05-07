using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetPlanner.Models
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }
        public virtual ICollection<BudgetAccount> BudgetAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public Household()
        {
            Users = new HashSet<ApplicationUser>();
            Invitations = new HashSet<Invitation>();
            BudgetAccounts = new HashSet<BudgetAccount>();
            Categories = new HashSet<Category>();
            BudgetItems = new HashSet<BudgetItem>();
        }
    }
}