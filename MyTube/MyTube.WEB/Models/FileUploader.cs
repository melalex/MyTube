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
            if (null == file)
            {
                return null;
            }

            if (!(file.ContentLength > 0))
            {
                return null;
            }

            string fileName = DateTime.Now.Millisecond + file.FileName;
            string fileExt = Path.GetExtension(file.FileName);

            if (null == fileExt)
            {
                return null;
            }

            if (!Directory.Exists(FilesPath))
            {
                Directory.CreateDirectory(FilesPath);
            }

            string path = Path.Combine(FilesPath, fileName);

            file.SaveAs(Path.GetFullPath(path));

            return path;
        }

        private static void RemoveFile(string path)
        {
            if (File.Exists(Path.GetFullPath(path)))
            {
                File.Delete(Path.GetFullPath(path));
            }
        }
    }
}