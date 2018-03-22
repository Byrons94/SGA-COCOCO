using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gestion_Activos.Startup))]
namespace Gestion_Activos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
