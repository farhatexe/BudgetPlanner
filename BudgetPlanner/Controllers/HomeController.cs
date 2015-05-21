using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetPlanner.Models;

namespace BudgetPlanner.Controllers
{   
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        [RequireHousehold]
        public ActionResult Index()
        {
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            ViewBag.hhName = db.Household.FirstOrDefault(h => h.Id == hhId).Name;

            var house = db.Household.FirstOrDefault(h => h.Id == hhId);

            var model = new DashboardViewModel()
            {
                Accounts = house.BudgetAccounts.ToList(),
                HouseholdUsers = house.Users.ToList(),
                Budgets = house.BudgetItems.ToList()
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}