﻿using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MyTube.BLL.Identity.Infrastructure;
using MyTube.DAL.Identity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.Identity.Interfaces
{
    public interface IIdentityServiceCreator
    {
        IIdentityService Create(string connectioString, IOwinContext context);
    }
}
