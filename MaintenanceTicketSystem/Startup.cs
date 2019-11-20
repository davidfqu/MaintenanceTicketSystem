using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MaintenanceTicketSystem.Startup))]
namespace MaintenanceTicketSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
