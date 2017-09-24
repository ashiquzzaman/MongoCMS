using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMS.WEB.Startup))]
namespace CMS.WEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
