using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StudentSquads.Startup))]
namespace StudentSquads
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
