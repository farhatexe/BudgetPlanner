using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetPlanner.Models;
using System.Data.Entity.SqlServer;

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
            var acctId = db.BudgetAccounts.FirstOrDefault(a => a.HouseholdId == hhId).Id;
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(-30);

            var house = db.Household.FirstOrDefault(h => h.Id == hhId);
            
            var model = new DashboardViewModel()
            {
                Accounts = house.BudgetAccounts.ToList(),
                HouseholdUsers = house.Users.ToList(),
                Transactions = db.Transactions.Where(t=> t.AccountId == acctId),
                Budgets = house.BudgetItems.ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult GetChartData()
        {
            // get household id and account id
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            var acctId = db.BudgetAccounts.FirstOrDefault(a => a.HouseholdId == hhId).Id;

            // get household
            var house = db.Household.FirstOrDefault(h => h.Id == hhId);

            // define starting date point
            var startPeriod = System.DateTime.Now.AddDays(-31);

            // get data for category labels, transaction amounts and budget amounts
            var data =
                (
                    from c in house.Categories
                    select new
                    {
                        Name = c.Name,
                        ActualAmount = (from t in c.Transactions                                    // get transactions for category
                                        where t.Date >= startPeriod                                 // that are within reporting period
                                         select t.AbsAmount).DefaultIfEmpty().Sum(),                // get amount and sum
                        BudgetAmount = c.BudgetItems.Select(t => t.Amount).DefaultIfEmpty().Sum()   // get budget mount for category
                    }
                );

            return Json(data);
        }

    }
}