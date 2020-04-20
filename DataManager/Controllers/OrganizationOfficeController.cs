using BusinessLogic;
using Catalogs;
using Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class OrganizationOfficeController : BaseController
    {
        public async Task<OrganizationOfficeModel> Get(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.GetOrganizationOffice(id);
        }
        public async Task<PaginatedResultModel<OrganizationOfficeModel>> GetPaginated(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, int organizationId, string name = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            OrganizationOfficeSearchModel filters = new OrganizationOfficeSearchModel();
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetOrganizationOffices(filters);
        }
        [HttpPost]
        public async Task<int> Create(OrganizationOfficeModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.CreateOrganizationOffice(model);
        }
        [HttpPost]
        public async Task<bool> Update(OrganizationOfficeModel model)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.UpdateOrganizationOffice(model);
        }
        [HttpDelete]
        public async Task<bool> Delete(int id)
        {
            var _logic = new Logic(LoggedInMemberId);
            return await _logic.DeleteOrganizationOffice(id);
        }

    }
}
