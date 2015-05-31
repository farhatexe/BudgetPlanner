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
    [RequireHousehold]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions

        [Route("Accounts/{acctId:int}/Transactions", Name="TransactionIndex")]
        public ActionResult Index(int acctId)
        {
            ViewBag.Balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
            ViewBag.reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).ReconciledBalance;
            ViewBag.Name = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Name;
            ViewBag.AccountId = acctId;

            var transactions = db.Transactions.Include(t => t.Category).Include(t=> t.UpdateByUser).Where(t=> t.AccountId == acctId);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        [Route("Accounts/{acctId:int}/Transactions/{id:int}", Name="TransactionDetails")]
        public ActionResult Details(int? id, int acctId)
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
        [Route("Accounts/{acctId:int}/Transactions/Create", Name = "TransactionCreate")]
        public ActionResult Create(int acctId)
        {
            ViewBag.Name = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Name;
            ViewBag.AcctId = acctId;
            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", acctId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Accounts/{acctId:int}/Transactions/Create")]
        public ActionResult Create([Bind(Include = "Id,AccountId,Amount,AbsAmount,Reconciled,AbsReconciledAmount,Date,Description,CategoryId")] Transaction transaction, int acctId)
        {
            if (ModelState.IsValid)
            {
                transaction.AccountId = acctId;
                transaction.AbsAmount = transaction.Amount;
                var catId = transaction.CategoryId;
                var exp = false;
                decimal reconBalance = 0;

                // determine if income or expense transaction 
                if (catId.HasValue)
                    exp = db.Categories.FirstOrDefault(c => c.Id == catId).Expense;

                // if expense and amount > 0, then reverse sign
                if (exp == true && transaction.Amount > 0)
                    transaction.Amount *= -1;

                // update balance
                var balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
                reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).ReconciledBalance;

                balance += transaction.Amount;

                // if transaction just reconciled, then calculate reconciled balance
                if (transaction.Reconciled == true)
                {
                    reconBalance += transaction.Amount;
                };

                // NEED TO UPDATE ACCOUNT WITH NEW BALANCE
                BudgetAccount acctToUpdate = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId);
                acctToUpdate.Balance = balance;
                acctToUpdate.ReconciledBalance = reconBalance;
                db.Entry(acctToUpdate).State = EntityState.Modified;

                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index", new { acctId });
            }

            ViewBag.Name = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Name;
            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", acctId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [Route("Accounts/{acctId:int}/Transactions/{id:int}/Edit", Name = "TransactionEdit")]
        public ActionResult Edit(int? id, int acctId)
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
        [Route("Accounts/{acctId:int}/Transactions/{id:int}/Edit")]
        public ActionResult Edit([Bind(Include = "Id,AccountId,Amount,AbsAmount,ReconciledAmount,Date,Description,CategoryId")] int acctId, Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var catId = transaction.CategoryId;
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

                transaction.AbsAmount = transaction.Amount;

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
                return RedirectToAction("Index", new { acctId });
            }

            ViewBag.Balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == transaction.AccountId).Balance;
            ViewBag.reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == transaction.AccountId).ReconciledBalance;
            ViewBag.Name = db.BudgetAccounts.FirstOrDefault(a => a.Id == transaction.AccountId).Name;
            ViewBag.AccountId = new SelectList(db.BudgetAccounts, "Id", "Name", acctId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [Route("Accounts/{acctId:int}/Transactions/{id:int}/Delete", Name = "TransactionDelete")]
        public ActionResult Delete(int? id, int acctId)
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

            ViewBag.AccountId = acctId;
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Accounts/{acctId:int}/Transactions/{id:int}/Delete")]
        public ActionResult DeleteConfirmed(int id, int acctId)
        {
            Transaction transaction = db.Transactions.Find(id);

            decimal amount = transaction.Amount * -1;

            // get account balance and reconciled balance
            decimal balance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).Balance;
            decimal reconBalance = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId).ReconciledBalance;

            // adjust balance
            balance += amount;
            reconBalance += amount;

            // update balance
            BudgetAccount acctToUpdate = db.BudgetAccounts.FirstOrDefault(a => a.Id == acctId);
            acctToUpdate.Balance = balance;

            if (transaction.Reconciled == true)
                acctToUpdate.ReconciledBalance = reconBalance;

            db.Entry(acctToUpdate).State = EntityState.Modified;            

            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index", new { acctId });
        }

        // datatable handler
        [HttpPost]
        [Route("Accounts/{acctId:int}/Transactions/Index", Name = "TransactionsTableAjax")]
        public JsonResult AjaxHandler([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest param, int acctId)
        {
            IQueryable<Transaction> filteredTransactions = db.Transactions.AsQueryable();

            var user = db.Users.Single(u => u.UserName == User.Identity.Name);
            var userId = User.Identity.GetUserId();

            filteredTransactions = filteredTransactions.Where(t => t.AccountId == acctId);

            var search = param.Search.Value;
            if (!string.IsNullOrEmpty(search))
            {
                //filteredTransactions = filteredTransactions
                //    .Where(t => t.Date.Equals(search) ||
                //        t.Description.Contains(search) ||
                //        t.Category.Name.Contains(search) ||
                //        t.Amount.Equals(search)  ||
                //        t.Reconciled.Equals(search)
                //        );
                filteredTransactions = filteredTransactions
                    .Where(t => t.Description.Contains(search) ||
                        t.Category.Name.Contains(search)
                        );
            }

            var column = param.Columns.FirstOrDefault(r => r.IsOrdered == true);
            if (column != null)
            {
                if (column.SortDirection == Column.OrderDirection.Descendant)
                {
                    switch (column.Data)
                    {
                        case "Date": filteredTransactions = filteredTransactions.OrderByDescending(t => t.Date);
                            break;
                        case "Description": filteredTransactions = filteredTransactions.OrderByDescending(t => t.Description);
                            break;
                        case "Category": filteredTransactions = filteredTransactions.OrderByDescending(t => t.Category.Name);
                            break;
                        case "Amount": filteredTransactions = filteredTransactions.OrderByDescending(t => t.Amount);
                            break;
                        case "Reconciled": filteredTransactions = filteredTransactions.OrderByDescending(t => t.Reconciled);
                            break;
                        case "UpdateByUser": filteredTransactions = filteredTransactions.OrderByDescending(t => t.UpdateByUser.Name);
                            break;
                    }
                }
                else
                {
                    switch (column.Data)
                    {
                        case "Date": filteredTransactions = filteredTransactions.OrderBy(t => t.Date);
                            break;
                        case "Description": filteredTransactions = filteredTransactions.OrderBy(t => t.Description);
                            break;
                        case "Category": filteredTransactions = filteredTransactions.OrderBy(t => t.Category.Name);
                            break;
                        case "Amount": filteredTransactions = filteredTransactions.OrderBy(t => t.Amount);
                            break;
                        case "Reconciled": filteredTransactions = filteredTransactions.OrderBy(t => t.Reconciled);
                            break;
                        case "UpdateByUser": filteredTransactions = filteredTransactions.OrderBy(t => t.UpdateByUser.Name);
                            break;
                    }
                }
            }
            var urlHelper = new UrlHelper(Request.RequestContext);
            var result = filteredTransactions.Skip(param.Start).Take(param.Length).ToList().Select(t => new TransactionViewModel() { 
                Date = t.Date.ToString("d"),
                Description = "<a href=\"" + urlHelper.RouteUrl("TransactionEdit", new { acctId = acctId, id = t.Id }) + "\">" + t.Description + "</a>",
                Amount = t.Amount,
                Category = t.Category.Name,
                Reconciled = t.Reconciled == true ? "Yes" : "No",
                UpdateBy = t.UpdateByUser != null ? t.UpdateByUser.Name : "",
                //Delete = "<a href=\"" + urlHelper.RouteUrl("TransactionDelete", new { id = t.Id }) + "\">" + class="glyphicon glyphicon-trash" + "</a>"
            });
            return Json(new DataTablesResponse(param.Draw, result, filteredTransactions.Count(), db.Transactions.Count()), JsonRequestBehavior.AllowGet);
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
