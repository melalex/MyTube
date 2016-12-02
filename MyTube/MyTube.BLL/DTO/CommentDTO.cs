using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.DTO
{
    class CommentDTO
    {
        public string Id { get; set; }
        public string CommentatorId { get; set; }
        public string CommentatorUsername { get; set; }
        public string CommentatorAvatarUri { get; set; }
        public DateTimeOffset CommentDateTime { get; set; }
        public string CommentText { get; set; }
    }
}
