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
    public class CampaignAttachmentController : BaseController
    {
        private readonly Logic _logic;

        public CampaignAttachmentController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("GetPaginated")]
        public async Task<PaginatedResultModel<AttachmentModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int campaignId, string orderByColumn = null, bool calculateTotal = true)
        {

            AttachmentSearchModel filters = new AttachmentSearchModel();
            filters.EntityId = campaignId;
            filters.EntityType = AttachmentEntityTypeCatalog.Campaign;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetAttachments(filters);
        }
        [HttpPut]
        [Route("Create")]
        public async Task<bool> Create(int campaignId, List<AttachmentModel> attachments)
        {
            return await _logic.AssignCampaignAttachments(campaignId, attachments);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> Delete(int id)
        {

            return await _logic.DeleteCampaignAttachment(id);
        }
    }
}