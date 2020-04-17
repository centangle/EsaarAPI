using BusinessLogic;
using Catalogs;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataManager.Controllers
{
    public class RegionController : BaseController
    {
        [HttpGet]
        public async Task<PaginatedResultModel<RegionBriefModel>> Countries(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = null;
            filters.Name = name;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetCountries(filters);
        }

        [HttpGet]
        public async Task<PaginatedResultModel<RegionBriefModel>> States(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name, int? countryId = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = countryId;
            filters.Name = name;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetStates(filters);
        }

        [HttpGet]
        public async Task<PaginatedResultModel<RegionBriefModel>> Districts(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name, int? stateId = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = stateId;
            filters.Name = name;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetDistricts(filters);
        }

        [HttpGet]
        public async Task<PaginatedResultModel<RegionBriefModel>> Tehsils(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name, int? districtId = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = districtId;
            filters.Name = name;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetTehsils(filters);
        }

        [HttpGet]
        public async Task<PaginatedResultModel<RegionBriefModel>> UnionCouncils(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name, int? tehsilId = null, string orderByColumn = null, bool calculateTotal = true)
        {
            var _logic = new Logic(LoggedInMemberId);
            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = tehsilId;
            filters.Name = name;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetUnionCouncils(filters);
        }

    }
}
