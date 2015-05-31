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

                
                //BudgetExpense = db.BudgetItems.Where(b => b.Category.Expense == true).Sum(a => a.Amount),
                //BudgetIncome = db.BudgetItems.Where(b => b.Category.Income == true).Sum(a => a.Amount)
            };

            

            return View(model);
        }

        [HttpPost]
        public JsonResult GetChartData()
        {
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            var acctId = db.BudgetAccounts.FirstOrDefault(a => a.HouseholdId == hhId).Id;
            //DateTime startDate = DateTime.Today;
            //DateTime endDate = DateTime.Today.AddDays(-30);

            var house = db.Household.FirstOrDefault(h => h.Id == hhId);

            // this needs to be set up in ajax call
            // get chart data
            var endPeriod = System.DateTime.Now.AddDays(31);
            var data =
                (
                    from c in house.Categories
                    select new
                    {
                        Name = c.Name,
                        ActualAmount = (from t in c.Transactions
                                        where t.Date <= endPeriod
                                         select t.AbsAmount).DefaultIfEmpty().Sum(),
                        //ActualAmount = c.Transactions.Where(t=> t.Date >= startDate && t.Date <= endDate).Select(t=> t.Amount).DefaultIfEmpty().Sum(),
                        BudgetAmount = c.BudgetItems.Select(t => t.Amount).DefaultIfEmpty().Sum()
                    }
                );

            return Json(data);
        }

    }
}