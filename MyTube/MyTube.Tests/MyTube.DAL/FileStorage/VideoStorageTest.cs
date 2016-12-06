using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MyTube.DAL.FileStorage;

namespace MyTube.Tests.MyTube.DAL.FileStorage
{
    [TestClass]
    public class VideoStorageTest
    {
        [TestMethod]
        public void FileStorage_Save_Saved()
        {
            // Arrange
            string path = @"C:\Users\Alexander\Desktop\20161126_202757.mp4";
            byte[] content = File.ReadAllBytes(path);
            LocalFileStorageFacade fileStorage = new LocalFileStorageFacade();

            // Act
            string fileUri = fileStorage.SaveVideo(content, "123");

            // Assert
            Assert.IsTrue(File.Exists(fileUri));
            File.Delete(fileUri);
        }
    }
}
