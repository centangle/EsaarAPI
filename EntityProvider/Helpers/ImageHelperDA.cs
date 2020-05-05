using Models.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Web.Hosting;

namespace EntityProvider.Helpers
{
    public static class ImageHelper
    {
        public static void Save(IImage model, string baseUrl)
        {
            if (!string.IsNullOrEmpty(model.ImageInBase64))
            {
                var imageParts = model.ImageInBase64.Split(',').ToList<string>();
                //Exclude the header from base64 by taking second element in List.
                byte[] ImageInBytes = Convert.FromBase64String(imageParts[1]);
                string filePath = "";
                if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    string tempFilePath = model.ImageUrl.TrimStart('/');
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
                filePath = "Uploads/Images/" + model.BaseFolder + "/" + Guid.NewGuid().ToString() + ".png";
                FileInfo file = new FileInfo(filePath);
                file.Directory.Create(); // If the directory already exists, this method does nothing.
                MemoryStream ms = new MemoryStream(ImageInBytes);
                Image Picture = Image.FromStream(ms);
                Picture.Save(filePath);
                model.ImageUrl = "/" + filePath;
            }
        }
    }
}
