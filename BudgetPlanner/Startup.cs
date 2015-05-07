using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BudgetPlanner.Startup))]
namespace BudgetPlanner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
