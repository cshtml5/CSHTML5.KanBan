using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SilverlightSignalRTest.Web.Startup))]
namespace SilverlightSignalRTest.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}