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

        public string SaveFile(byte[] fileContent, string fileName)
        {
            string path = $@"{Directory.GetCurrentDirectory()}\{storageFolder}\{fileName}";
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            File.WriteAllBytes(path, fileContent);
            return $@"{storageFolder.Replace('\\','/')}/{fileName}";
        }
    }
}
