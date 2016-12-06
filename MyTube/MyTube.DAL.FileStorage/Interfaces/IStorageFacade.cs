using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    public interface IStorageFacade
    {
        string DefaultAvatarUri { get; }

        Task<string> DefaultPosterUriAsync(string filePath);

        string SaveAvatar(byte[] fileContent, string fileName);

        string SavePoster(byte[] fileContent, string fileName);

        Task<string> SaveVideoAsync(string filePath);

        void DeleteAvatar(string uri);

        void DeletePoster(string uri);

        void DeleteVideo(string uri);
    }
}
