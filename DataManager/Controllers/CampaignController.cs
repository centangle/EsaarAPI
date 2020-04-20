using BusinessLogic;
using Catalogs;
using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class CampaignController : BaseController
    {
        public async Task<CampaignModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetCampaign(id);
        }
        public async Task<PaginatedResultModel<CampaignModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int? organizationId = null, int? eventId = null, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            CampaignSearchModel filters = new CampaignSearchModel();
            filters.Name = name;
            filters.OrganizationId = organizationId;
            filters.EventId = eventId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetCampaigns(filters);
        }
        [HttpPost]
        public async Task<int> Create(CampaignModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateCampaign(model);
        }
        [HttpPost]
        public async Task<bool> Update(CampaignModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateCampaign(model);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteCampaign(id);
        }

    }
}
