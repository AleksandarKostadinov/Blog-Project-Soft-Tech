using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogProject.Startup))]
namespace BlogProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
