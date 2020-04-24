using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataManager.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public async Task Index()
        {
            await new Logic().GetAllRegionsInRadius(33.521527, 73.1741041, 10, Catalogs.RegionSearchTypeCatalog.Intersects, Catalogs.RegionRadiusTypeCatalog.Kilometers);
        }
    }
}