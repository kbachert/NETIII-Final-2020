using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCPresentationLayer.Startup))]
namespace MVCPresentationLayer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
