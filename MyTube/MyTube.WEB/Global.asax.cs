using Microsoft.AspNet.Identity;
using MyTube.WEB.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyTube.WEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CacheResetScheduler.Start();
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom.Equals("User", StringComparison.InvariantCultureIgnoreCase) && User.Identity.IsAuthenticated)
            {
                return "User=" + User.Identity.GetUserId();
            }

            return base.GetVaryByCustomString(context, custom);
        }
    }
}
