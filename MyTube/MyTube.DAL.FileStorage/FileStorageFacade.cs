using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.FileStorage.Interfaces;
using MyTube.DAL.FileStorage.Repositories;

namespace MyTube.DAL.FileStorage
{
    public class FileStorageFacade : IStorageFacade
    {
        private IFileRepository videoStorage;
        private IFileRepository avatarsStorage;
        private IFileRepository posterStorage;

        public FileStorageFacade()
        {
            avatarsStorage = new FielRepository(@"Images\Avatars");
            posterStorage = new FielRepository(@"Images\Posters");
            videoStorage = new FielRepository(@"Videos");
        }

        public Uri SaveAvatar(byte[] fileContent, string fileName, string fileExtension)
        {
            return avatarsStorage.SaveFile(fileContent, fileName, fileExtension);
        }

        public Uri SavePoster(byte[] fileContent, string fileName, string fileExtension)
        {
            return posterStorage.SaveFile(fileContent, fileName, fileExtension);
        }

        public Uri SaveVideo(byte[] fileContent, string fileName, string fileExtension)
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
