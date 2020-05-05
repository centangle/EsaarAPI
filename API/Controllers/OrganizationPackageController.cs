using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationPackageController : BaseController
    {
        private readonly Logic _logic;

        public OrganizationPackageController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPackage")]
        public async Task<PackageModel> GetPackage(int id)
        {
            
            return await _logic.GetPackage(id);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(PackageModel model)
        {
            
            return await _logic.AddPackage(model);
        }
        [HttpPost]
        [Route("Update")]
        public async Task<bool> Update(PackageModel model)
        {
            
            return await _logic.UpdatePackage(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            
            return await _logic.DeletePackage(id);
        }
    }
}