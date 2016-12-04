using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.DTO
{
    public class SubscriptionDTO
    {
        public string Id { get; set; }
        public string Publisher { get; set; }
        public string Subscriber { get; set; }
        public DateTimeOffset StartDate { get; set; }
    }
}
