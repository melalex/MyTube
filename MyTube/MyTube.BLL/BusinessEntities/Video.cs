using MyTube.BLL.DTO;
using MyTube.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.BLL.BusinessEntities
{
    public class Video
    {
        private IUnitOfWork database;

        public Video(IUnitOfWork database)
        {
            this.database = database;
        }

        public string Id { get; set; }
        public string UploderUsername { get; set; }
        public string UploderAvatarUri { get; set; }
        public string Name { get; set; }
        public string VideoUri { get; set; }
        public string VideoCategory { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; } 

        public async IEnumerable<CommentDTO> Comments()
        {
            var comments = database.Comments.GetAll();
            return from comment in comments
                   where comment.DestinationVideoIdString == Id
                   let commentator = await database.Channels.Get(comment.ComentatorIdString)
                   select new CommentDTO
                   {
                       Id = comment.Id,
                       CommentatorId = "",
                       CommentatorUsername = "",
                       CommentatorAvatarUri = "",
                       CommentDateTime =,
                       CommentText = "",
                   };
        }
    }
}
