using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.SwaggerFilters;
using BusinessLogic;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentController : BaseController
    {
        private readonly Logic _logic;
        private readonly IWebHostEnvironment _environment;

        public AttachmentController(Logic logic, IWebHostEnvironment environment)
        {
            _logic = logic;
            _environment = environment;
        }
        public class FileUploadModel
        {
            public IFormFile file { get; set; }
        }
        [HttpPost]
        [SwaggerFileOperationFilter.FileContentType]
        [Route("Upload")]
        [Produces("application/json")]
        public async Task<string> Upload([FromForm(Name = "File")]IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var rootPath = Path.Combine(_environment.ContentRootPath, "Uploads");
                    var fileRootPath = Path.Combine(rootPath, "Attachments");
                    var name = file.FileName;
                    var fileExtension = name.Split('.').Last();
                    var originalFileName = Path.GetFileNameWithoutExtension(name);
                    var systemFileName = Guid.NewGuid().ToString() + "." + fileExtension;
                    var fileFullPath = Path.Combine(fileRootPath, systemFileName);
                    Directory.CreateDirectory(fileRootPath);
                    using (FileStream fileStream = System.IO.File.Create(fileFullPath))
                    {
                        file.CopyTo(fileStream);
                        await fileStream.FlushAsync();
                    }
                    var attachment = new AttachmentModel()
                    {
                        Url = @"/Uploads/Attachments/" + systemFileName,
                        SystemFileName = systemFileName,
                        OriginalFileName = originalFileName,
                        FileExtension = fileExtension,
                    };
                    await _logic.CreateAttachment(attachment);
                    return attachment.Url;
                }
                else
                {
                    throw new KnownException("Attachments could not be uploaded. Check your MIME type");
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(string url)
        {
            try
            {
                var rootPath = _environment.ContentRootPath;
                var fileDirectory = url.Replace("/", @"\");
                string filePath = Path.GetFullPath(Path.Combine(rootPath + fileDirectory));

                if (await _logic.DeleteAttachment(url))
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;


        }

    }
}