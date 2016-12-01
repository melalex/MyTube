using MyTube.DAL.FileStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTube.DAL.FileStorage.Repositories
{
    public class FielRepository : IFileRepository
    {
        private string storageFolder;

        public FielRepository(string storageFolder)
        {
            this.storageFolder = storageFolder;
        }

        public void DeleteFile(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Uri SaveFile(byte[] fileContent, string fileName, string fileExtension)
        {
            string path = $@"{Directory.GetCurrentDirectory()}\{storageFolder}\{fileName}.{fileExtension}";
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            File.WriteAllBytes(path, fileContent);
            return new Uri(path);
        }
    }
}
