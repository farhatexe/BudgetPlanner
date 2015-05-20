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
        public ActionResult Dashboard()
        {
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            var hhName = db.Household.FirstOrDefault(h => h.Id == hhId).Name;
            ViewBag.Name = hhName;

            var household = db.Household.Select(h => h.Id == hhId);
            return View(household);
        }

        [RequireHousehold]
        public ActionResult Index()
        {
            ViewBag.Title = "Household";
            return View();
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