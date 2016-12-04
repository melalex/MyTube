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

        string DefaultPosterUri { get; }

        string SaveAvatar(byte[] fileContent, string fileName);

        string SavePoster(byte[] fileContent, string fileName);

        string SaveVideo(byte[] fileContent, string fileName);

        void DeleteAvatar(string uri);

        void DeletePoster(string uri);

        void DeleteVideo(string uri);
    }
}
