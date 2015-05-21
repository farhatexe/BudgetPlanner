using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BudgetPlanner.Models;

namespace BudgetPlanner.Controllers
{   
    [Authorize]
    [RequireHousehold]
    public class BudgetAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetAccounts
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var hhId = int.Parse(User.Identity.GetHouseholdId());

            var budgetAccounts = db.BudgetAccounts.Include(b => b.Household);
            ViewBag.Household = db.Household.First(h => h.Id == hhId).Name;

            return View(budgetAccounts.ToList());
        }

        // GET: BudgetAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetAccount budgetAccount = db.BudgetAccounts.Find(id);
            if (budgetAccount == null)
            {
                return HttpNotFound();
            }
            return View(budgetAccount);
        }

        // GET: BudgetAccounts/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var hhId = int.Parse(User.Identity.GetHouseholdId());

            ViewBag.Household = db.Household.First(h => h.Id == hhId).Name;
            return View();
        }

        // POST: BudgetAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Balance,ReconciledBalance")] BudgetAccount budgetAccount)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                budgetAccount.HouseholdId = int.Parse(User.Identity.GetHouseholdId());

                db.BudgetAccounts.Add(budgetAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", budgetAccount.HouseholdId);
            return View(budgetAccount);
        }

        // GET: BudgetAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetAccount budgetAccount = db.BudgetAccounts.Find(id);
            if (budgetAccount == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            var hhId = int.Parse(User.Identity.GetHouseholdId());

            ViewBag.Household = db.Household.First(h => h.Id == hhId).Name;
            return View(budgetAccount);
        }

        // POST: BudgetAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name,Balance,ReconciledBalance")] BudgetAccount budgetAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", budgetAccount.HouseholdId);
            return View(budgetAccount);
        }

        // GET: BudgetAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetAccount budgetAccount = db.BudgetAccounts.Find(id);
            if (budgetAccount == null)
            {
                return HttpNotFound();
            }
            return View(budgetAccount);
        }

        // POST: BudgetAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Delete Transactions
            //Transaction transactions = db.Transactions.SelectMany(t=> t.AccountId)
            // Delete account
            BudgetAccount budgetAccount = db.BudgetAccounts.Find(id);
            db.BudgetAccounts.Remove(budgetAccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
