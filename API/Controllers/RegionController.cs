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
using Models.BriefModel;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegionController : BaseController
    {
        private readonly Logic _logic;

        public RegionController(Logic logic)
        {
            _logic = logic;
        }
        [HttpGet]
        [Route("Countries")]
        public async Task<PaginatedResultModel<RegionBriefModel>> Countries(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, int? organizationId = null, string orderByColumn = null, bool calculateTotal = true)
        {

            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = null;
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetCountries(filters);
        }

        [HttpGet]
        [Route("States")]
        public async Task<PaginatedResultModel<RegionBriefModel>> States(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, int? countryId = null, int? organizationId = null, string orderByColumn = null, bool calculateTotal = true)
        {

            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = countryId;
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetStates(filters);
        }

        [HttpGet]
        [Route("Districts")]
        public async Task<PaginatedResultModel<RegionBriefModel>> Districts(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, int? stateId = null, int? organizationId = null, string orderByColumn = null, bool calculateTotal = true)
        {

            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = stateId;
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetDistricts(filters);
        }

        [HttpGet]
        [Route("Tehsils")]
        public async Task<PaginatedResultModel<RegionBriefModel>> Tehsils(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, int? districtId = null, int? organizationId = null, string orderByColumn = null, bool calculateTotal = true)
        {

            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = districtId;
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetTehsils(filters);
        }

        [HttpGet]
        [Route("UnionCouncils")]
        public async Task<PaginatedResultModel<RegionBriefModel>> UnionCouncils(int recordsPerPage, int currentPage, PaginationOrderCatalog orderDir, bool disablePagination, string name = null, int? tehsilId = null, int? organizationId = null, string orderByColumn = null, bool calculateTotal = true)
        {

            RegionSearchModel filters = new RegionSearchModel();
            filters.ParentId = tehsilId;
            filters.Name = name;
            filters.OrganizationId = organizationId;
            SetPaginationProperties(filters, recordsPerPage, currentPage, orderDir, orderByColumn, disablePagination, calculateTotal);
            return await _logic.GetUnionCouncils(filters);
        }

        [HttpGet]
        [Route("Levels")]
        public List<BaseBriefModel> Levels()
        {
            List<BaseBriefModel> levels = new List<BaseBriefModel>();
            foreach (RegionLevelTypeCatalog regionLevel in Enum.GetValues(typeof(RegionLevelTypeCatalog)))
            {
                levels.Add(new BaseBriefModel
                {
                    Id = (int)regionLevel,
                    Name = regionLevel.ToString()
                });
            }
            return levels;
        }

        [HttpGet]
        [Route("GetRadiusType")]
        public Array GetRadiusType()
        {
            return Enum.GetValues(typeof(RegionRadiusTypeCatalog)).Cast<RegionRadiusTypeCatalog>().ToArray();
        }

        [HttpGet]
        [Route("GetRegionSearchType")]
        public Array GetRegionSearchType()
        {
            return Enum.GetValues(typeof(RegionSearchTypeCatalog)).Cast<RegionSearchTypeCatalog>().ToArray();
        }

    }
}