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
        private readonly Logic _logic;

        public UOMController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<PaginatedResultModel<UOMModel>> Get(string name = null, bool forDropDown = false)
        {
            
            if (forDropDown)
                return await _logic.GetUOMForDD(new UOMSearchModel { Name = name });
            else
                return await _logic.GetUOM(new UOMSearchModel { Name = name });
        }
        [HttpGet]
        [Route("Get/{id:int}")]
        public async Task<UOMModel> Get(int id)
        {
            
            return await _logic.GetUOM(id);
        }
        [HttpGet]
        [Route("GetSingleUOMTree")]
        public async Task<IEnumerable<UOMModel>> GetSingleUOMTree(int id, DataStructureCatalog dataStructure)
        {
            
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetSingleUOMHierarchy(id, getHierarichalData);
        }
        [HttpGet]
        [Route("GetPeripheralUOMs")]
        public async Task<IEnumerable<UOMModel>> GetPeripheralUOMs()
        {
            
            return await _logic.GetPeripheralUOMs();
        }
        [HttpGet]
        [Route("GetRootUOMs")]
        public async Task<IEnumerable<UOMModel>> GetRootUOMs()
        {
            
            return await _logic.GetRootUOMs();
        }
        [HttpGet]
        [Route("GetAllUOMs")]
        public async Task<IEnumerable<UOMModel>> GetAllUOMs(DataStructureCatalog dataStructure)
        {
            
            bool getHierarichalData = (dataStructure == DataStructureCatalog.List ? false : true);
            return await _logic.GetAllUOMs(getHierarichalData);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(UOMModel model)
        {
            
            return await _logic.CreateUOM(model);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<bool> Update(UOMModel model)
        {
            
            return await _logic.UpdateUOM(model);
        }
        [HttpPost]
        [Route("CreateSingleUOMWithChildrens")]
        public async Task<bool> CreateSingleUOMWithChildrens(UOMModel model)
        {
            
            return await _logic.CreateSingleUOMWithChildrens(model);
        }
        [HttpPut]
        [Route("UpdateSingleUOMWithChildren")]
        public async Task<bool> UpdateSingleUOMWithChildren(UOMModel model)
        {
            
            return await _logic.UpdateSingleUOMWithChildren(model);
        }
        [HttpPost]
        [Route("CreateMultipleUOMsWithChildrens")]
        public async Task<bool> CreateMultipleUOMsWithChildrens(List<UOMModel> uOMs)
        {
            
            return await _logic.CreateMultipleUOMsWithChildrens(uOMs);
        }
        [HttpPut]
        [Route("UpdateMultipleUOMsWithChildrens")]
        public async Task<bool> UpdateMultipleUOMsWithChildrens(List<UOMModel> uOMs)
        {
            
            return await _logic.UpdateMultipleUOMsWithChildrens(uOMs);
        }
        [HttpDelete]
        [Route("DeleteUOMWithChildren")]
        public async Task<bool> DeleteUOMWithChildren(int id)
        {
            
            return await _logic.DeleteUOM(id);
        }
    }
}