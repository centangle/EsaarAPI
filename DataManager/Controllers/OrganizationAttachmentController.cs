using BusinessLogic;
using Catalogs;
using Models;
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
    public class OrganizationAttachmentController : BaseController
    {
        [HttpGet]
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
        public async Task<bool> Create(int organizationId, List<AttachmentModel> attachments)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.AssignOrganizationAttachments(organizationId, attachments);
        }
    }
}
