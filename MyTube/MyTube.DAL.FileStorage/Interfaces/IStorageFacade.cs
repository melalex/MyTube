using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    interface IStorageFacade
    {
        Uri SaveAvatar(byte[] fileContent, string fileName, string fileExtension);

        Uri SavePoster(byte[] fileContent, string fileName, string fileExtension);

        Uri SaveVideo(byte[] fileContent, string fileName, string fileExtension);

        void DeleteAvatar(Uri uri);

        void DeletePoster(Uri uri);

        void DeleteVideo(Uri uri);
    }
}
