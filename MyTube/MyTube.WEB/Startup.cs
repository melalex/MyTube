using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyTube.WEB.Startup))]
namespace MyTube.WEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
