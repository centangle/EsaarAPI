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
        public async Task<OrganizationAccountModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganizationAccount(id);
        }
        public async Task<PaginatedResultModel<OrganizationAccountModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationAccountSearchModel filters = new OrganizationAccountSearchModel();
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationAccounts(filters);
        }
        [HttpPost]
        public async Task<int> Create(OrganizationAccountModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateOrganizationAccount(model);
        }
        [HttpPost]
        public async Task<bool> Update(OrganizationAccountModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateOrganizationAccount(model);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationAccount(id);
        }
    }
}