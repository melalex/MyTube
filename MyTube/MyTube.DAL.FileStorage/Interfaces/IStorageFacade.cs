using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    public interface IStorageFacade
    {
        void SetStorageFolder(string storageFolder);

        string DefaultAvatarUri { get; }

        string DefaultPosterUriAsync(string filePath);

        Task<string> SaveAvatar(string filePath);

        Task<string> SavePoster(string filePath);

        string SaveVideoAsync(string filePath);

        void DeleteAvatar(string uri);

        void DeletePoster(string uri);

        void DeleteVideo(string uri);
    }
}
