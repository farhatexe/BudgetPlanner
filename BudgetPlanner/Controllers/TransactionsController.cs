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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Category);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,Amount,AbsAmount,ReconciledAmount,AbsReconciledAmount,Date,Description,CategoryId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var catId = transaction.CategoryId;
                var acctId = transaction.AccountId;
                var dc = false;

                // determine if income or expense transaction 
                if (catId.HasValue)
                    dc = db.BudgetItems.FirstOrDefault(c => c.CategoryId == catId).Classification;

                // if expense and amount > 0, then reverse sign
                if (dc = false && transaction.Amount > 0)
                    transaction.Amount *= -1;

                // update balance
                var balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
                balance += transaction.Amount;

                // NEED TO UPDATE ACCOUNT WITH NEW BALANCE

                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            TempData["Transaction"] = transaction;

            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Amount,AbsAmount,ReconciledAmount,AbsReconciledAmount,Date,Description,CategoryId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var catId = transaction.CategoryId;
                var acctId = transaction.AccountId;
                var dc = false;
                var oldAmount = 0;

                // get tempdata amount
                //if(TempData["Transaction"] != null)
                //    Transaction oldTransaction = (Transaction)TempData["Transaction"];

                // determine if transaction amount changed and if so, reverse sign
                //if(oldTransaction.Amount != transaction.Amount)
                //    oldAmount = oldTransaction.Amount * -1;

                // determine if income or expense transaction 
                if (catId.HasValue)
                    dc = db.BudgetItems.FirstOrDefault(c => c.CategoryId == catId).Classification;

                // if expense and amount > 0, then reverse sign
                if (dc = false && transaction.Amount > 0)
                    transaction.Amount *= -1;

                // get account balance and update first with oldAmount and then new amount
                var balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
                balance += oldAmount;
                balance += transaction.Amount;

                // NEED TO UPDATE ACCOUNT WITH NEW BALANCE

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
