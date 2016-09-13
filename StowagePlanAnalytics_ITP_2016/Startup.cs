using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StowagePlanAnalytics_ITP_2016.Startup))]
namespace StowagePlanAnalytics_ITP_2016
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
