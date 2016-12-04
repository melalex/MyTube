using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.DTO
{
    public class NotificationDTO
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public DateTimeOffset NotificationDateTime { get; set; }
    }
}
