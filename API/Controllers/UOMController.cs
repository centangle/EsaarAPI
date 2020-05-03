using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic;
using Catalogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UOMController : BaseController
    {
        [HttpGet]
        public async Task<PaginatedResultModel<UOMModel>> Get(string name = null, bool forDropDown = false)
        {
            var _logic = new Logic(LoggedInMemberId);
            if (forDropDown)
                return await _logic.GetUOMForDD(new UOMSearchModel { Name = name });
            else
                return await _logic.GetUOM(new UOMSearchModel { Name = name });
        }
        [HttpGet]
        public async Task<UOMModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetUOM(id);
        }
        [HttpGet]
        public async Task<IEnumerable<UOMModel>> GetSingleUOMTree(int id, DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleUOMHierarchy(id, getHierarichalData);
        }
        [HttpGet]
        public async Task<IEnumerable<UOMModel>> GetPeripheralUOMs()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetPeripheralUOMs();
        }
        [HttpGet]
        public async Task<IEnumerable<UOMModel>> GetRootUOMs()
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetRootUOMs();
        }
        [HttpGet]
        public async Task<IEnumerable<UOMModel>> GetAllUOMs(DataStructureCatalog dataStructure)
        {
            var _logic = new Logic(LoggedInMemberId);
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllUOMs(getHierarichalData);
        }
        [HttpPost]
        public async Task<int> Create(UOMModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateUOM(model);
        }
        [HttpPut]
        public async Task<bool> Update(UOMModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateUOM(model);
        }
        [HttpPost]
        public async Task<bool> CreateSingleUOMWithChildrens(UOMModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateSingleUOMWithChildrens(model);
        }
        [HttpPut]
        public async Task<bool> UpdateSingleUOMWithChildren(UOMModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateSingleUOMWithChildren(model);
        }
        [HttpPost]
        public async Task<bool> CreateMultipleUOMsWithChildrens(List<UOMModel> uOMs)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateMultipleUOMsWithChildrens(uOMs);
        }
        [HttpPut]
        public async Task<bool> UpdateMultipleUOMsWithChildrens(List<UOMModel> uOMs)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateMultipleUOMsWithChildrens(uOMs);
        }
        [HttpDelete]
        public async Task<bool> DeleteUOMWithChildren(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteUOM(id);
        }
    }
}