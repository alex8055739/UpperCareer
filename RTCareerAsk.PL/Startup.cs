using Microsoft.Owin;
using Owin;
using AVOSCloud;

[assembly: OwinStartupAttribute(typeof(RTCareerAsk.PL.Startup))]
namespace RTCareerAsk.PL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AVClient.Initialize("5ptNj5fF9TplwYYNYo34Ujmi-gzGzoHsz", "oxEMyVyz3XmlI8URg87Xp1l5");
            ConfigureAuth(app);
        }
    }
}
