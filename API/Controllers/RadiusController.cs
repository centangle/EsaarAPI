using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RadiusController : ControllerBase
    {
        private readonly Logic _logic;
        private readonly IEmailSender _emailSender;

        public RadiusController(Logic logic,IEmailSender emailSender)
        {
            _logic = logic;
            _emailSender = emailSender;
        }
        [HttpGet]
        public async Task<FilteredRegionsModel> Radius()
        {
            return await _logic.GetAllRegionsInRadius(33.521527, 73.1741041, 10, Catalogs.RegionSearchMethodCatalog.Intersects, Catalogs.RegionRadiusTypeCatalog.Kilometers);
        }
        [HttpPost]
        public async Task SendEmail()
        {
            await _emailSender.SendEmailAsync("safi.centangle@gmail.com", "Test Email", "<h1>Email From .NET CORE</h1>");
        }
    }
}