using MyTube.BLL.DTO;
using MyTube.DAL.Interfaces;
using MyTube.DAL.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.Entities;

namespace MyTube.BLL.BusinessEntities
{
    public class VideoProxy
    {
        private IUnitOfWork database;

        internal Video video { get; private set; }

        private VideoProxy()
        {
            
        }

        public static async Task<VideoProxy> Create(IUnitOfWork database, Video video)
        {
            VideoProxy thisVideo = new VideoProxy();
            thisVideo.database = database;
            thisVideo.video = video;

            Channel uploder = await database.Channels.Get(video.UploderIdString);
            
            thisVideo.UploderAvatarUri = uploder.AvatarUri;
            thisVideo.UploderUsername = uploder.Username;
            
            return thisVideo;
        }


        public string UploderUsername { get; private set; }
        public string UploderAvatarUri { get; private set; }

        public string Id
        {
            get
            {
                return video.IdString;
            }
        }

        public string Name
        {
            get
            {
                return video.Name;
            }
            set
            {
                video.Name = value;
            }
        }

        public DateTimeOffset Uploaded
        {
            get
            {
                return video.UploadDate;
            }
        }

        public string VideoUri
        {
            get
            {
                return video.VideoUrl;
            }
            set
            {
                video.VideoUrl = value;
            }
        }

        public string PosterUri
        {
            get
            {
                return video.PosterUrl;
            }
            set
            {
                video.PosterUrl = value;
            }
        }

        public string VideoCategory
        {
            get
            {
                return video.Category;
            }
            set
            {
                video.Category = value;
            }
        }

        public List<string> Tags
        {
            get
            {
                return video.Tags;
            }
            set
            {
                video.Tags = value;
            }
        }

        public string Description
        {
            get
            {
                return video.Description;
            }
            set
            {
                video.Description = value;
            }
        }

        public int Likes
        {
            get
            {
                return video.Likes;
            }
            set
            {
                video.Likes = value;
            }
        }

        public int Dislikes
        {
            get
            {
                return video.Dislikes;
            }
            set
            {
                video.Dislikes = value;
            }
        }

        public int Views
        {
            get
            {
                return video.Views;
            }
            set
            {
                video.Views = Views;
            }
        } 

        public async Task<IEnumerable<CommentDTO>> CommentsAsync(int skip, int limit)
        {
            IEnumerable<Comment> comments = await database.Comments.GetCommentsFromVideoAsync(video, skip, limit);
            
            var tasks =  comments.Select(async x => {
                Channel commentator = await database.Channels.Get(x.ComentatorIdString);
                return new CommentDTO
                {
                    Id = x.IdString,
                    VideoId = this.Id,
                    CommentatorAvatarUri = commentator.AvatarUri,
                    CommentatorUsername = commentator.Username,
                    CommentatorId = commentator.IdString,
                    CommentDateTime = x.CommentDateTime,
                    CommentText = x.Text,
                };
            }).ToList();
            return await Task.WhenAll(tasks);
        }
    }
}
