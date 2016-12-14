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
        public string DefaultAvatarUri { get; private set; }

        private string storageFolder;

        private string defaultAvatarPath = String.Format("{0}Images{0}Avatars{0}default_user_image.gif", Path.DirectorySeparatorChar);
        private string avatarsStoragePath = String.Format("{0}Images{0}Avatars", Path.DirectorySeparatorChar);
        private string posterStoragePath = String.Format("{0}Images{0}Posters", Path.DirectorySeparatorChar);
        private string videoStoragePath = Path.DirectorySeparatorChar + "Videos";

        private IFileRepository videoStorage;
        private IFileRepository avatarsStorage;
        private IFileRepository posterStorage;

        public void SetStorageFolder(string storageFolder)
        {
            this.storageFolder = storageFolder;
            DefaultAvatarUri = (storageFolder + defaultAvatarPath).Replace(Path.DirectorySeparatorChar, '/');
            avatarsStorage = new LocalFileRepository(storageFolder + avatarsStoragePath);
            posterStorage = new LocalFileRepository(storageFolder + posterStoragePath);
            videoStorage = new LocalFileRepository(storageFolder + videoStoragePath);
        }

        public string DefaultPosterUriAsync(string filePath)
        {
            //return new Task<string>(() =>
            //{
            //    string fileName = Path.GetFileNameWithoutExtension(filePath);
            //    var ffMpeg = new FFMpegConverter();
            //    using (Stream posterStream = posterStorage.SaveFileStream(fileName, ".jpg"))
            //    {
            //        ffMpeg.GetVideoThumbnail(filePath, posterStream);
            //    }
            //    return $@"/Uploads/Files{posterStoragePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}.jpg";
            //});

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            var ffMpeg = new FFMpegConverter();
            using (Stream posterStream = posterStorage.SaveFileStream(fileName, ".jpg"))
            {
                ffMpeg.GetVideoThumbnail(filePath, posterStream);
            }
            return $@"/Uploads/Files{posterStoragePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}.jpg";
        }

        public async Task<string> SaveAvatar(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            using (Stream avatarStream = avatarsStorage.SaveFileStream(fileName, fileExtension))
            using (Stream sourceAvatarStream = File.Open(filePath, FileMode.Open))
            {
                await sourceAvatarStream.CopyToAsync(avatarStream);
            }
            return $@"/Uploads/Files{avatarsStoragePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}{fileExtension}";
        }

        public async Task<string> SavePoster(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);
            using (Stream posterStream = avatarsStorage.SaveFileStream(fileName, fileExtension))
            using (Stream sourcePosterStream = File.Open(filePath, FileMode.Open))
            {
                await sourcePosterStream.CopyToAsync(posterStream);
            }
            return $@"/Uploads/Files{posterStoragePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}{fileExtension}";
        }

        public string SaveVideoAsync(string filePath)
        {
            //return new Task<string>(() =>
            //{
            //    var ffMpeg = new FFMpegConverter();
            //    string fileName = Path.GetFileNameWithoutExtension(filePath);
            //    using (Stream streamMP4 = videoStorage.SaveFileStream(fileName, ".mp4"))
            //    using (Stream streamOGG = videoStorage.SaveFileStream(fileName, ".ogg"))
            //    using (Stream streamWEBM = videoStorage.SaveFileStream(fileName, ".webm"))
            //    {
            //        ffMpeg.ConvertMedia(filePath, streamMP4, Format.mp4);
            //        ffMpeg.ConvertMedia(filePath, streamOGG, Format.ogg);
            //        ffMpeg.ConvertMedia(filePath, streamWEBM, Format.webm);
            //        return $@"/Uploads/Files{videoStoragePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}";
            //    }
            //});
            var ffMpeg = new FFMpegConverter();
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            using (Stream streamMP4 = videoStorage.SaveFileStream(fileName, ".mp4"))
            using (Stream streamOGG = videoStorage.SaveFileStream(fileName, ".ogg"))
            using (Stream streamWEBM = videoStorage.SaveFileStream(fileName, ".webm"))
            {
                ffMpeg.ConvertMedia(filePath, streamMP4, Format.mp4);
                ffMpeg.ConvertMedia(filePath, streamOGG, Format.ogg);
                ffMpeg.ConvertMedia(filePath, streamWEBM, Format.webm);
                return $@"/Uploads/Files{videoStoragePath.Replace(Path.DirectorySeparatorChar, '/')}/{fileName}";
            }
        }

        public void DeleteAvatar(string uri)
        {
            string fileName = uri.Split('/').LastOrDefault();
            avatarsStorage.DeleteFile(fileName);
        }

        public void DeletePoster(string uri)
        {
            string fileName = uri.Split('/').LastOrDefault();
            posterStorage.DeleteFile(fileName);
        }

        public void DeleteVideo(string uri)
        {
            string fileName = uri.Split('/').LastOrDefault();
            videoStorage.DeleteFile(fileName);
        }
    }
}
