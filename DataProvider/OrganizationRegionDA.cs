using Catalogs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<Array> GetOrganizationRegionAllowedLevels(int organizationId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                int maxRegionLevel = 0;
                var queryableOrgRegions = context.EntityRegions.Where(x => x.EntityId == organizationId
                  && x.EntityType == (int)EntityRegionTypeCatalog.Organization
                  && x.IsActive == true
                  && x.IsApproved == true
                  && x.IsDeleted == false
                  ).AsQueryable();

                try
                {
                    maxRegionLevel = await queryableOrgRegions.MaxAsync(x => x.RegionLevel);
                    return Enum.GetValues(typeof(RegionLevelTypeCatalog))
                    .Cast<RegionLevelTypeCatalog>()
                    .Where(x => (int)x <= maxRegionLevel).ToArray();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
