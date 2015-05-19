using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using System.Web.Routing;

namespace BudgetPlanner.Models
{
    public static class AuthExtensions
    {

     public static async Task RefreshAuthentication(this HttpContext context, ApplicationUser user)
     {
         context.GetOwinContext().Authentication.SignOut();
         await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
     }

        public static string GetHouseholdId(this IIdentity user)
        {
            var claimsIdentity = (ClaimsIdentity)user;
            var HouseholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            if (HouseholdClaim != null)
                return HouseholdClaim.Value;
            else
                return null;
        }

        
    }
    public class RequireHousehold : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                var isAuthorized = base.AuthorizeCore(httpContext);
                if (!isAuthorized)
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(httpContext.User.Identity.GetHouseholdId()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        //}

        //public class RequireHousehold : AuthorizeAttribute
        //{
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {

                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "Household", action = "Create" }));
                }
            }
        }
}