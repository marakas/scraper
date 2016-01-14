using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(sfdb.Startup))]
namespace sfdb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
