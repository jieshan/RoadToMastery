using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RoadToMastery.Startup))]
namespace RoadToMastery
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
