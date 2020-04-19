using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<PaginatedResultModel<PaginatedEntityRegionModel>> GetEntityRegions(EntityRegionSearchModel filters)
        {
            return await _dataAccess.GetEntityRegions(filters);
        }
        public async Task<bool> ModifyMultipleEntityRegion(List<EntityRegionModel> entityRegions, int organizationId, int? requestId)
        {
            return await _dataAccess.ModifyMultipleEntityRegion(entityRegions, organizationId, requestId);
        }
    }
}
