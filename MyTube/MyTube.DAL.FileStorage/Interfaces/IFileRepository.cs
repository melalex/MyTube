using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    interface IFileRepository
    {
        Uri SaveFile(byte[] fileContent, string fileName, string fileExtension);
        void DeleteFile(Uri uri);
    }
}
