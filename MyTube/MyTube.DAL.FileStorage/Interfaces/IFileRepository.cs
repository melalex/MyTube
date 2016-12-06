using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    public interface IFileRepository
    {
        Stream SaveFileStream(string fileName, string extension);
        void DeleteFile(string uri);
    }
}
