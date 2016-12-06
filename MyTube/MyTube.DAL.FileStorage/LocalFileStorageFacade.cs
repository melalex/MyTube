using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTube.DAL.FileStorage.Interfaces;
using MyTube.DAL.FileStorage.Repositories;
using NReco.VideoConverter;
using System.IO;

namespace MyTube.DAL.FileStorage
{
    public class LocalFileStorageFacade : IStorageFacade
    {
        private const string _defaultAvatarUri = "Images/Avatars/default_user_image.gif";
        private const string avatarsStoragePath = @"Images\Avatars";
        private const string posterStoragePath = @"Images\Posters";
        private const string videoStoragePath = @"Videos";

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

        public Task<string> DefaultPosterUriAsync(string filePath)
        {
            return new Task<string>(() =>
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                Stream posterStream = posterStorage.SaveFileStream(fileName, "jpg");
                var ffMpeg = new FFMpegConverter();
                ffMpeg.GetVideoThumbnail(filePath, posterStream);
                return $@"{posterStoragePath.Replace('\\', '/')}/{fileName}.jpg";
            });
        }

        public string SaveAvatar(byte[] fileContent, string fileName)
        {
#warning SaveAvatar is not working
            Stream avatarStream = avatarsStorage.SaveFileStream(fileName, fileName);
            return $@"{avatarsStoragePath.Replace('\\', '/')}/{fileName}.jpg";
        }

        public string SavePoster(byte[] fileContent, string fileName)
        {
#warning SavePoster is not working
            return "";
        }

        public Task<string> SaveVideoAsync(string filePath)
        {
            return new Task<string>(() =>
            {
                var ffMpeg = new FFMpegConverter();
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                Stream streamMP4 = videoStorage.SaveFileStream(fileName, "mp4");
                Stream streamOGG = videoStorage.SaveFileStream(fileName, "ogg");
                Stream streamWEBM = videoStorage.SaveFileStream(fileName, "webm");
                ffMpeg.ConvertMedia(filePath, streamMP4, Format.mp4);
                ffMpeg.ConvertMedia(filePath, streamOGG, Format.ogg);
                ffMpeg.ConvertMedia(filePath, streamWEBM, Format.webm);
                return $@"{videoStoragePath.Replace('\\', '/')}/{fileName}";
            });
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
