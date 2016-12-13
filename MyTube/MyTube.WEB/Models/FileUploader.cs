using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyTube.WEB.Models
{
    public static class FileUpload
    {
        private static string FilesPath = HttpContext.Current.Server.MapPath("~\\Temp");

        public static string UploadFile(HttpPostedFileBase file)
        {
            // Check if we have a file
            if (null == file)
            {
                return null;
            }

            // Make sure the file has content
            if (!(file.ContentLength > 0))
            {
                return null;
            }

            string fileName = DateTime.Now.Millisecond + file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            // Make sure we were able to determine a proper extension
            if (null == fileExt)
            {
                return null;
            }

            // Check if the directory we are saving to exists
            if (!Directory.Exists(FilesPath))
            {
                // If it doesn't exist, create the directory
                Directory.CreateDirectory(FilesPath);
            }

            // Set our full path for saving
            string path = FilesPath + Path.DirectorySeparatorChar + fileName;

            // Save our file
            file.SaveAs(Path.GetFullPath(path));

            // Return the filename
            return path;
        }

        private static void RemoveFile(string path)
        {
            // Check if our file exists
            if (File.Exists(Path.GetFullPath(path)))
            {
                // Delete our file
                File.Delete(Path.GetFullPath(path));
            }
        }
    }
}