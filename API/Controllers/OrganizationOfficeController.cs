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
    public class OrganizationOfficeController : BaseController
    {
        private readonly Logic _logic;

        public OrganizationOfficeController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<OrganizationOfficeModel> Get(int id)
        {
            
            return await _logic.GetOrganizationOffice(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationOfficeModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            
            OrganizationOfficeSearchModel filters = new OrganizationOfficeSearchModel();
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationOffices(filters);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(OrganizationOfficeModel model)
        {
            
            return await _logic.CreateOrganizationOffice(model);
        }
        [HttpPost]
        [Route("Update")]
        public async Task<bool> Update(OrganizationOfficeModel model)
        {
            
            return await _logic.UpdateOrganizationOffice(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            
            return await _logic.DeleteOrganizationOffice(id);
        }
    }
}