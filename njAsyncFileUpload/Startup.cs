using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(njAsyncFileUpload.Startup))]
namespace njAsyncFileUpload
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
