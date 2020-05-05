using Catalogs;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<PaginatedResultModel<RegionBriefModel>> GetCountries(RegionSearchModel filters)
        {
            return await _dataAccess.GetCountries(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetStates(RegionSearchModel filters)
        {
            return await _dataAccess.GetStates(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetDistricts(RegionSearchModel filters)
        {
            return await _dataAccess.GetDistricts(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetTehsils(RegionSearchModel filters)
        {
            return await _dataAccess.GetTehsils(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetUnionCouncils(RegionSearchModel filters)
        {
            return await _dataAccess.GetUnionCouncils(filters);
        }
        public async Task<RadiusRegionsModel> GetAllRegionsInRadius(double latitude, double longitude, float radius, RegionSearchTypeCatalog searchType, RegionRadiusTypeCatalog radiusType)
        {
            return await _dataAccess.GetAllRegionsInRadius(latitude, longitude, radius, searchType, radiusType);
        }
    }
}
