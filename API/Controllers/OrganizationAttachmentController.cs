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
    public class OrganizationAttachmentController : BaseController
    {
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<AttachmentModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            AttachmentSearchModel filters = new AttachmentSearchModel();
            filters.EntityId = organizationId;
            filters.EntityType = Catalogs.AttachmentEntityTypeCatalog.Organization;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetAttachments(filters);
        }
        [HttpPut]
        [Route("Create")]
        public async Task<bool> Create(int organizationId, List<AttachmentModel> attachments)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignOrganizationAttachments(organizationId, attachments);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationAttachment(id);
        }
    }
}