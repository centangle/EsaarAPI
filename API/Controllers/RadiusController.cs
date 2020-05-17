using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
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

        public RadiusController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        public async Task<FilteredRegionsModel> Radius()
        {
            return await _logic.GetAllRegionsInRadius(33.521527, 73.1741041, 10, Catalogs.RegionSearchMethodCatalog.Intersects, Catalogs.RegionRadiusTypeCatalog.Kilometers);
        }
    }
}