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
using System.ComponentModel;

namespace BudgetPlanner.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index([DefaultValue(1)]int? acctId)
        //public ActionResult Index()
        {
            ViewBag.Balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
            ViewBag.reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).ReconciledBalance;
            ViewBag.Name = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Name;
            ViewBag.AccountId = acctId;

            var transactions = db.Transactions.Include(t => t.Category).Include(t=> t.UpdateByUser).Where(t=> t.AccountId == acctId);
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
            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name");
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
                var exp = false;

                // determine if income or expense transaction 
                if (catId.HasValue)
                    exp = db.Categories.FirstOrDefault(c => c.Id == catId).Expense;

                // if expense and amount > 0, then reverse sign
                if (exp == true && transaction.Amount > 0)
                    transaction.Amount *= -1;

                // update balance
                var balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
                balance += transaction.Amount;

                // NEED TO UPDATE ACCOUNT WITH NEW BALANCE
                BudgetAccount acctToUpdate = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId);
                acctToUpdate.Balance = balance;
                db.Entry(acctToUpdate).State = EntityState.Modified;

                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", transaction.AccountId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id, [DefaultValue(1)] int? acctId)
        //public ActionResult Edit(int? id)
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

            ViewBag.Balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
            ViewBag.reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).ReconciledBalance;
            ViewBag.Name = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Name;
            ViewBag.AccountId = acctId;
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Amount,AbsAmount,ReconciledAmount,Date,Description,CategoryId,Reconciled")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var catId = transaction.CategoryId;
                var acctId = transaction.AccountId;
                var exp = false;
                decimal balance = 0;
                decimal reconBalance = 0;
                decimal oldAmount = 0;
                decimal amtDifference = 0;

                Transaction oldTransaction = new Transaction();

                // get tempdata amount
                if (TempData["Transaction"] != null)
                {
                    oldTransaction = TempData["Transaction"] as Transaction;
                }                   

                // determine if transaction amount changed and if so, reverse sign
                if (oldTransaction.Amount != transaction.Amount)
                {
                    amtDifference = amtDifference + (oldTransaction.Amount - transaction.Amount);
                    oldAmount = oldTransaction.Amount * -1;
                }

                // determine if income or expense transaction 
                if (catId.HasValue)
                    exp = db.Categories.FirstOrDefault(c => c.Id == catId).Expense;

                // if expense and amount > 0, then reverse sign
                if (exp == true && transaction.Amount > 0)
                    transaction.Amount *= -1;

                // get account balance and reconciled balance
                balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
                reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).ReconciledBalance;

                // calculate balance if transaction amount changed and not reconciled
                if(oldTransaction.Amount != transaction.Amount)
                {
                    balance += oldAmount; 
                    balance += transaction.Amount;
                }

                // if transaction just reconciled, then calculate reconciled balance
                if (transaction.Reconciled == true)
                {
                    reconBalance += oldAmount;
                    reconBalance += transaction.Amount;
                };

                // NEED TO UPDATE ACCOUNT WITH NEW BALANCE
                BudgetAccount acctToUpdate = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId);
                acctToUpdate.Balance = balance;
                acctToUpdate.ReconciledBalance = reconBalance;

                db.Entry(acctToUpdate).State = EntityState.Modified;

                transaction.UpdateByUserId = userId;
                transaction.Updated = DateTimeOffset.Now;

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

            // DO I NEED TO CHECK FOR RECONSILED FLAG?
            // IF NOT RECONCILED, SHOULD AMOUNT BE ADDED BACK TO BALANCE?
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
