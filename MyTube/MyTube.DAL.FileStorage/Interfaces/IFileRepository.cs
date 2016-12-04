using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Interfaces
{
    public interface IFileRepository
    {
        string SaveFile(byte[] fileContent, string fileName);
        void DeleteFile(string uri);
    }
}
