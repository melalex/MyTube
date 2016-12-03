using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    public interface IStorageFacade
    {
        string DefaultAvatarUri();

        string DefaultPosterUri();

        string SaveAvatar(byte[] fileContent, string fileName, string fileExtension);

        string SavePoster(byte[] fileContent, string fileName, string fileExtension);

        string SaveVideo(byte[] fileContent, string fileName, string fileExtension);

        void DeleteAvatar(Uri uri);

        void DeletePoster(Uri uri);

        void DeleteVideo(Uri uri);
    }
}
