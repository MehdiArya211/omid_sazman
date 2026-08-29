using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace VisitorManagment.Core.Classes
{
    public class FileUploadCheck
    {

        public static bool CheckFileExtension(IFormFile imageName)
        {
            if (imageName == null)
                return true;

            string[] permittedExtensions = { ".doc",".docx",".txt",".pdf",".ttf",".bmp",".ico", ".svg" ,".jpeg", ".jpg", ".png" , ".ico" , ".svg", ".tif" };

            var ext = Path.GetExtension(imageName.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }

        //Upload Avatar
        public static bool CheckImageFileExtension(IFormFile imageName)
        {
            if (imageName == null)
                return true;

            string[] permittedExtensions = {".jpeg", ".jpg", ".png"};

            var ext = Path.GetExtension(imageName.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }

    }

}
