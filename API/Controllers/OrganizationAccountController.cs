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
    public class OrganizationAccountController : BaseController
    {
        private readonly Logic _logic;

        public OrganizationAccountController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Get")]
        public async Task<OrganizationAccountModel> Get(int id)
        {
            
            return await _logic.GetOrganizationAccount(id);
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<OrganizationAccountModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            
            OrganizationAccountSearchModel filters = new OrganizationAccountSearchModel();
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationAccounts(filters);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<int> Create(OrganizationAccountModel model)
        {
            
            return await _logic.CreateOrganizationAccount(model);
        }
        [HttpPost]
        [Route("Update")]
        public async Task<bool> Update(OrganizationAccountModel model)
        {
            
            return await _logic.UpdateOrganizationAccount(model);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            
            return await _logic.DeleteOrganizationAccount(id);
        }
    }
}