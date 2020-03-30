using Models.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Web.Hosting;

namespace DataProvider.Helpers
{
    public static class ImageHelper
    {
        public static void Save(IImage model)
        {
            if (!string.IsNullOrEmpty(model.ImageInBase64))
            {
                var ImageInBytes = Convert.FromBase64String(model.ImageInBase64);
                string imageUrl = model.ImageUrl;
                if (string.IsNullOrEmpty(imageUrl))
                {
                    var basePath = HostingEnvironment.MapPath("~");
                    var imagePath = "/Images/" + model.BaseFolder + "/" + DateTime.Now.Ticks.ToString() + ".png";
                    model.ImageUrl = imagePath;
                    imageUrl = basePath + imagePath;
                }
                FileInfo file = new FileInfo(imageUrl);
                file.Directory.Create(); // If the directory already exists, this method does nothing.
                if (File.Exists(imageUrl))
                {
                    File.Delete(imageUrl);
                }
                MemoryStream ms = new MemoryStream(ImageInBytes);
                Image Picture = Image.FromStream(ms);
                Picture.Save(imageUrl);
            }
        }
    }
}
