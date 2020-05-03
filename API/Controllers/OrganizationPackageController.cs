﻿using System;
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
        [HttpGet]
        [Route("GetPackage")]
        public async Task<PackageModel> GetPackage(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetPackage(id);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(PackageModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AddPackage(model);
        }
        [HttpPost]
        [Route("Update")]
        public async Task<bool> Update(PackageModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdatePackage(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeletePackage(id);
        }
    }
}