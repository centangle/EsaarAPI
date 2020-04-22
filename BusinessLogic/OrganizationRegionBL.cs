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
        public async Task<List<BaseBriefModel>> GetOrganizationRegionAllowedLevels(int organizationId)
        {
            return await _dataAccess.GetOrganizationRegionAllowedLevels(organizationId);
        }
    }
}
