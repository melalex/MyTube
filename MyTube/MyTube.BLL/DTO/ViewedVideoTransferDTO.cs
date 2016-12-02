using MyTube.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.DTO
{
    public class ViewedVideoTransferDTO
    {
        public string Id { get; set; }
        public DateTimeOffset ViewDateTime { get; set; }
        public ViewStatus Status { get; set; }
        public string VideoId { get; set; }
    }
}
