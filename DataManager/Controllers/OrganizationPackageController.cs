using BusinessLogic;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    [Authorize]
    public class OrganizationPackageController : BaseController
    {
        [HttpGet]
        public async Task<PackageModel> GetPackage(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetPackage(id);
        }
        [HttpPost]
        public async Task<int> Create(PackageModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AddPackage(model);
        }
        [HttpPost]
        public async Task<bool> Update(PackageModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdatePackage(model);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeletePackage(id);
        }

    }
}
