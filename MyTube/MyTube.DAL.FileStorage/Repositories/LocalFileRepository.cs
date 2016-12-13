using MyTube.DAL.FileStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Repositories
{
    public class LocalFileRepository : IFileRepository
    {
        private string storageFolder;

        public LocalFileRepository(string storageFolder)
        {
            this.storageFolder = storageFolder;
        }

        public void DeleteFile(string uri)
        {
            throw new NotImplementedException();
        }

        public Stream SaveFileStream(string fileName, string extension)
        {
            string path = $@"{storageFolder}\{fileName}.{extension}";
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            return File.Create(path);
        }
    }
}
