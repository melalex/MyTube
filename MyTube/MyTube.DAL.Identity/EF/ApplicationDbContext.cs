using Microsoft.AspNet.Identity.EntityFramework;
using MyTube.DAL.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.Identity.EF
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private const string defaultConnectioString = @"Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\aspnet-MyTube.WEB-20161127063542.mdf;Initial Catalog=aspnet-MyTube.WEB-20161127063542;Integrated Security=True";
         
        public ApplicationDbContext()
            : base(defaultConnectioString, throwIfV1Schema: false)
        {
        }

        public ApplicationDbContext(string connectionString)
            : base(connectionString, throwIfV1Schema: false)
        {
        }
    }
}
