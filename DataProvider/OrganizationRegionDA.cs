using Catalogs;
using Models.BriefModel;
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
        public async Task<List<BaseBriefModel>> GetOrganizationRegionAllowedLevels(int organizationId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                int minRegionLevel = 0;
                var queryableOrgRegions = context.EntityRegions.Where(x =>
                  x.EntityId == organizationId
                  && x.EntityType == (int)EntityRegionTypeCatalog.Organization
                  && x.IsActive == true
                  && x.IsApproved == true
                  && x.IsDeleted == false
                  ).AsQueryable();

                try
                {
                    minRegionLevel = await queryableOrgRegions.MinAsync(x => x.RegionLevel);
                    var allowedLevels = Enum.GetValues(typeof(RegionLevelTypeCatalog))
                    .Cast<RegionLevelTypeCatalog>()
                    .Where(x => (int)x >= minRegionLevel).ToArray();
                    List<BaseBriefModel> levels = new List<BaseBriefModel>();
                    foreach (RegionLevelTypeCatalog regionLevel in allowedLevels)
                    {
                        levels.Add(new BaseBriefModel
                        {
                            Id = (int)regionLevel,
                            Name = regionLevel.ToString()
                        });
                    }
                    return levels;

                }
                catch (Exception ex)
                {
                    return new List<BaseBriefModel>();
                }
            }
        }
    }
}
