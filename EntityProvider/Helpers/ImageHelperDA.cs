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
                //var ImageInBytes = Convert.FromBase64String(model.ImageInBase64);
                if (string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = "Images/" + model.BaseFolder + "/" + DateTime.Now.Ticks.ToString() + ".png";
                    FileInfo file = new FileInfo(model.ImageUrl);
                    file.Directory.Create(); // If the directory already exists, this method does nothing.
                    if (File.Exists(model.ImageUrl))
                    {
                        File.Delete(model.ImageUrl);
                    }
                }
                else
                {
                    model.ImageUrl = model.ImageUrl.Split('/')[1];
                }

                MemoryStream ms = new MemoryStream(ImageInBytes);
                Image Picture = Image.FromStream(ms);
                Picture.Save(model.ImageUrl);
                model.ImageUrl = "/" + model.ImageUrl;
            }
        }
    }
}
