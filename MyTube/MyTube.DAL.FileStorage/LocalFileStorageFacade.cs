using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.FileStorage.Interfaces;
using MyTube.DAL.FileStorage.Repositories;

namespace MyTube.DAL.FileStorage
{
    public class LocalFileStorageFacade : IStorageFacade
    {
        private const string _defaultAvatarUri = "Images/Avatars/default_user_image.gif";
        private const string _defaultPosterUri = "Images/Posters/default_poster_image.gif";

        private IFileRepository videoStorage;
        private IFileRepository avatarsStorage;
        private IFileRepository posterStorage;

        public LocalFileStorageFacade()
        {
            avatarsStorage = new LocalFileRepository(@"Images\Avatars");
            posterStorage = new LocalFileRepository(@"Images\Posters");
            videoStorage = new LocalFileRepository(@"Videos");
        }

        public string DefaultAvatarUri
        {
            get
            {
                return _defaultAvatarUri;
            }
        }

        public string DefaultPosterUri
        {
            get
            {
                return _defaultPosterUri;
            }
        }

        public string SaveAvatar(byte[] fileContent, string fileName)
        {
            return avatarsStorage.SaveFile(fileContent, fileName);
        }

        public string SavePoster(byte[] fileContent, string fileName)
        {
            return posterStorage.SaveFile(fileContent, fileName);
        }

        public string SaveVideo(byte[] fileContent, string fileName)
        {
            return videoStorage.SaveFile(fileContent, fileName);
        }

        public void DeleteAvatar(string uri)
        {
            avatarsStorage.DeleteFile(uri);
        }

        public void DeletePoster(string uri)
        {
            posterStorage.DeleteFile(uri);
        }

        public void DeleteVideo(string uri)
        {
            videoStorage.DeleteFile(uri);
        }
    }
}
