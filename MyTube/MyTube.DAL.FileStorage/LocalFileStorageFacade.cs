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
        private IFileRepository videoStorage;
        private IFileRepository avatarsStorage;
        private IFileRepository posterStorage;

        public LocalFileStorageFacade()
        {
            avatarsStorage = new LocalFileRepository(@"Images\Avatars");
            posterStorage = new LocalFileRepository(@"Images\Posters");
            videoStorage = new LocalFileRepository(@"Videos");
        }

        public string DefaultAvatarUri()
        {
            return "Images/Avatars/default_user_image.gif";
        }

        public string DefaultPosterUri()
        {
            return "Images/Posters/default_poster_image.gif";
        }

        public string SaveAvatar(byte[] fileContent, string fileName, string fileExtension)
        {
            return avatarsStorage.SaveFile(fileContent, fileName, fileExtension);
        }

        public string SavePoster(byte[] fileContent, string fileName, string fileExtension)
        {
            return posterStorage.SaveFile(fileContent, fileName, fileExtension);
        }

        public string SaveVideo(byte[] fileContent, string fileName, string fileExtension)
        {
            return videoStorage.SaveFile(fileContent, fileName, fileExtension);
        }

        public void DeleteAvatar(Uri uri)
        {
            avatarsStorage.DeleteFile(uri);
        }

        public void DeletePoster(Uri uri)
        {
            posterStorage.DeleteFile(uri);
        }

        public void DeleteVideo(Uri uri)
        {
            videoStorage.DeleteFile(uri);
        }
    }
}
