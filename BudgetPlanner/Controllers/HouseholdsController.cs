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
using System.Threading.Tasks;
using System.Configuration;
using SendGrid;
using System.Net.Mail;
using System.IO;

namespace BudgetPlanner.Controllers
{
    [Authorize]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households/Select
        [Authorize]
        public ActionResult Select()
        {
            ViewBag.Join = "";
            ViewBag.Create = "";
            return View();
        }


        [RequireHousehold]
        public ActionResult Display()
        {
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            var hhName = db.Household.FirstOrDefault(h => h.Id == hhId).Name;
            ViewBag.Name = hhName;

            Household household = db.Household.Find(hhId);
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            var userId = User.Identity.GetUserId();
            var hhId = User.Identity.GetHouseholdId();
            var name = form["Name"];

            if (hhId == "")
            {
                var household = new Household
                {
                    Name = name
                };
                db.Household.Add(household);
                db.SaveChanges();

                var user = db.Users.Find(userId);
                user.HouseholdId = db.Household.FirstOrDefault(h=> h.Name == name).Id;

                db.SaveChanges();

                // refresh cookie
                await ControllerContext.HttpContext.RefreshAuthentication(user);

                return RedirectToAction("Index","Home");
            }

            ViewBag.Create = "You are already in a Household";
            return RedirectToAction("Select");
        }

        // POST: Households/Invite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(FormCollection form)
        {
            var email = form["Email"];
            var userId = User.Identity.GetUserId();
            var hhId = int.Parse(User.Identity.GetHouseholdId());
            var hhName = db.Household.FirstOrDefault(h => h.Id == hhId).Name;

            var invitation = new Invitation
            {
                FromUserId = userId,
                ToEmail = email,
                HouseholdId = hhId
            };

            db.Invitations.Add(invitation);
            db.SaveChanges();

            // send invitation email
            var myAddress = ConfigurationManager.AppSettings["ContactEmail"];
            var myUserName = ConfigurationManager.AppSettings["UserName"];
            var myPassword = ConfigurationManager.AppSettings["Password"];
            var link = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;

            SendGridMessage mail = new SendGridMessage();
            mail.From = new MailAddress(myAddress);
            mail.AddTo(email);
            mail.Subject = "Invitation to join Household";
            mail.Text = "You have been invited to join the " + hhName + " household Budget Planner. Click this link " + link + " to register";
            var credentials = new NetworkCredential(myUserName, myPassword);
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(mail);

            
            return RedirectToAction("Display", "Households");

        }   

        // POST: Households/Leave
        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Leave()
        {

            var userId = User.Identity.GetUserId();

            // update user profile to remove household id
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
            user.HouseholdId = null;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // refresh cookie to remove household
            var removeuser = db.Users.Find(User.Identity.GetUserId());
            await ControllerContext.HttpContext.RefreshAuthentication(removeuser);

            return RedirectToAction("Select", "Households");

        }

        
        // POST: Households/Join
        [HttpPost, ActionName("Join")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Join(FormCollection form)
        {
            // Need to check Invitations for user email address
            // if found, then update user with Household

            var email = form["Email"];
            var userId = User.Identity.GetUserId();
            var hhId = db.Invitations.FirstOrDefault(i => i.ToEmail == email).HouseholdId;

            if(hhId != 0)
            {
                // update user household id
                ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == userId);
                user.HouseholdId = hhId;
                db.Entry(user).State = EntityState.Modified;

                // remove invitation
                Invitation userInvitation = db.Invitations.FirstOrDefault(i => i.ToEmail == email);
                db.Invitations.Remove(userInvitation);
                db.SaveChanges();

                // refresh cookie to add household
                var adduser = db.Users.Find(User.Identity.GetUserId());
                await ControllerContext.HttpContext.RefreshAuthentication(adduser);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Join = "No Invitation was found for this Email";
            ViewBag.Create = "";
            return RedirectToAction("Select");
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
