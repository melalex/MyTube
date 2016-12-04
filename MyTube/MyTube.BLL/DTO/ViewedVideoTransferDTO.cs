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

        public string Viewer { get; set; }

        public string ViewedVideo { get; set; }

        public ViewStatus Status { get; set; }

        public DateTimeOffset ShowDateTime { get; set; }
    }
}
