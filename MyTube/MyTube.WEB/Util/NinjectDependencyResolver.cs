using MyTube.BLL.Identity.Interfaces;
using MyTube.BLL.Identity.Services;
using MyTube.BLL.Interfaces;
using MyTube.BLL.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyTube.WEB.Util
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            kernel.Bind<IIdentityServiceCreator>().To<IdentityServiceCreator>();
            kernel.Bind<IUserService>().To<UserService>().OnActivation(x => 
                x.SetStorageFolder(HttpContext.Current.Server.MapPath(@"~\Files"))
            );
        }
    }
}