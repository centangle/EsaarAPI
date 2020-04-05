using BusinessLogic;
using DataManager.UploadHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class AttachmentController : BaseController
    {
        [HttpPost]
        public async Task<string> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/Attachments");
            Directory.CreateDirectory(root);
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.FileData)
                {
                    var name = file.Headers.ContentDisposition.FileName;
                    // Remove Double Quotes From Name
                    name = name.Trim('"');

                    var fileExtension = name.Split('.').Last();
                    var fileName = Path.GetFileNameWithoutExtension(name);
                    var originalFileName = fileName;
                    var systemFileName = Guid.NewGuid().ToString() + "." + fileExtension;
                    var filePath = Path.Combine(root, systemFileName);
                    File.Move(file.LocalFileName, filePath);
                    var attachment = new AttachmentModel()
                    {
                        Url = @"/Attachments/" + systemFileName,
                        SystemFileName = systemFileName,
                        OriginalFileName = originalFileName,
                        FileExtension = fileExtension,
                    };
                    var _logic = new Logic(LoggedInMemberId);
                    await _logic.CreateAttachment(attachment);
                    return attachment.Url;
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }
        [HttpDelete]
        public async Task<bool> Delete(string url)
        {
            try
            {
                var ctx = HttpContext.Current;
                var root = ctx.Server.MapPath("~");
                var fileDirectory = url.Replace("/", @"\");
                string filePath = Path.GetFullPath(Path.Combine(root + fileDirectory));
                var _logic = new Logic(LoggedInMemberId);
                if (await _logic.DeleteAttachment(url))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;


        }

        //public async Task<HttpResponseMessage> Upload()
        //{
        //    // Check if the request contains multipart/form-data.  
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
        //    //access form data  
        //    NameValueCollection formData = provider.FormData;
        //    //access files  
        //    IList<HttpContent> files = provider.Files;

        //    HttpContent file1 = files[0];
        //    //var thisFileName = file1.Headers.ContentDisposition.FileName.Trim('\"');

        //    ////-------------------------------------For testing----------------------------------  
        //    //to append any text in filename.  
        //    var thisFileName = file1.Headers.ContentDisposition.FileName.Trim('\"') + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //ToDo: Uncomment this after UAT as per Jeeevan  

        //    //List<string> tempFileName = thisFileName.Split('.').ToList();  
        //    //int counter = 0;  
        //    //foreach (var f in tempFileName)  
        //    //{  
        //    //    if (counter == 0)  
        //    //        thisFileName = f;  

        //    //    if (counter > 0)  
        //    //    {  
        //    //        thisFileName = thisFileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + f;  
        //    //    }  
        //    //    counter++;  
        //    //}  

        //    ////-------------------------------------For testing----------------------------------  

        //    string filename = String.Empty;
        //    Stream input = await file1.ReadAsStreamAsync();
        //    string directoryName = String.Empty;
        //    string URL = String.Empty;
        //    string tempDocUrl = ConfigurationManager.AppSettings["UploadUrl"];

        //    if (formData["ClientDocs"] == "ClientDocs")
        //    {
        //        var path = HttpRuntime.AppDomainAppPath;
        //        directoryName = System.IO.Path.Combine(path, "ClientDocument");
        //        filename = System.IO.Path.Combine(directoryName, thisFileName);

        //        //Deletion exists file  
        //        if (File.Exists(filename))
        //        {
        //            File.Delete(filename);
        //        }

        //        string DocsPath = tempDocUrl + "/" + "ClientDocument" + "/";
        //        URL = DocsPath + thisFileName;

        //    }


        //    //Directory.CreateDirectory(@directoryName);  
        //    using (Stream file = File.OpenWrite(filename))
        //    {
        //        input.CopyTo(file);
        //        //close file  
        //        file.Close();
        //    }

        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    response.Headers.Add("DocsUrl", URL);
        //    return response;
        //}

    }
}
