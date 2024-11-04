using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Grievancemis.Startup))]
namespace Grievancemis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
