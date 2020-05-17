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
        public async Task<List<ItemBriefModel>> GetRootCategoriesByOrganization(int organizationId)
        {
            return await _dataAccess.GetRootCategoriesByOrgOrCampaign(organizationId);
        }
    }
}
