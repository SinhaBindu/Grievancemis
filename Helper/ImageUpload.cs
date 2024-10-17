using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Grievancemis.Healper
{
    public class ImageResult
    {
        public bool Success { get; set; }
        public string ImageName { get; set; }
        public string ErrorMessage { get; set; }

    }
    public class ImageUpload
    {
        public ImageResult UploadFile(HttpPostedFileBase file, string UploadPath,string EmpCode)
        {
            ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };
            if (file != null)
            {
                var path = UploadPath;
                string extension = Path.GetExtension(file.FileName);
                if (!ValidateExtension(extension))
                {
                    imageResult.Success = false;
                    imageResult.ErrorMessage = "Invalid Extension";
                    return imageResult;
                }
                try
                {
                    string PicName = EmpCode+"_"+DateTime.Now.ToString("ddMMyyhhmmssff");
                    var versions = new Dictionary<string, string>();
                    versions.Add("_small", "maxwidth=150&maxheight=100&format=jpg");
                    //versions.Add("_medium", "maxwidth=900&maxheight=900&format=jpg");
                    //versions.Add("_large", "maxwidth=1200&maxheight=1200&format=jpg");
                    foreach (var suffix in versions.Keys)
                    {
                        file.InputStream.Seek(0, SeekOrigin.Begin);
                        ImageBuilder.Current.Build(new ImageJob(
                                file.InputStream,
                                path + PicName,//file.FileName,
                                new Instructions(versions[suffix]),
                                false,
                                true));
                    }
                    imageResult.ImageName =PicName + ".jpg";//file.FileName;
                    return imageResult;
                }
                catch (Exception ex)
                {
                    imageResult.Success = false;
                    imageResult.ErrorMessage = ex.Message;
                    return imageResult;
                }
            }
            imageResult.ImageName = "NoImage.jpg";
            return imageResult;
        }
        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".gif":
                    return true;
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }
    }
}