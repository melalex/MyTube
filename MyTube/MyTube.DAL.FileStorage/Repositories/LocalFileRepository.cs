﻿using MyTube.DAL.FileStorage.Interfaces;
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

        public void DeleteFile(string fileName)
        {
            string path = Path.Combine(storageFolder, fileName);
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }            
        }

        public Stream SaveFileStream(string fileName, string extension)
        {
            string path = Path.Combine(storageFolder, fileName + extension);

            if (!Directory.Exists(storageFolder))
            {
                Directory.CreateDirectory(storageFolder);
            }

            return File.Create(path);
        }
    }
}
