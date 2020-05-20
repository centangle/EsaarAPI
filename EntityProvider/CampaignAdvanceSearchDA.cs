using Catalogs;
using EntityProvider.DbModels;
using EntityProvider.Helpers;
using Models;
using Models.BriefModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<PaginatedResultModel<CampaignModel>> GetCampaigns(CampaignSearchModel filters)
        {
            var campaignQueryable = (from c in _context.Campaigns
                                     where (string.IsNullOrEmpty(filters.Name) || c.Name.Contains(filters.Name) || c.NativeName.Contains(filters.Name))
                                     && (filters.OrganizationId == null || c.OrganizationId == filters.OrganizationId)
                                     && (filters.EventId == null || c.EventId == filters.EventId)
                                     && c.IsDeleted == false
                                     select c).AsQueryable();



            var filteredQueryable = await ApplyAdvanceFilters(campaignQueryable, filters);

            var selectionQueryable = (from c in filteredQueryable
                                      join o in _context.Organizations on c.OrganizationId equals o.Id
                                      join e in _context.Events on c.EventId equals e.Id into le
                                      from e in le.DefaultIfEmpty()
                                      where (string.IsNullOrEmpty(filters.Name) || c.Name.Contains(filters.Name) || c.NativeName.Contains(filters.Name))
                                      && (filters.OrganizationId == null || c.OrganizationId == filters.OrganizationId)
                                      && (filters.EventId == null || c.EventId == filters.EventId)
                                      && c.IsDeleted == false
                                      select new CampaignModel
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          NativeName = c.NativeName,
                                          Description = c.Description,
                                          ImageUrl = c.ImageUrl,
                                          IsActive = c.IsActive,
                                          StartDate = c.StartDate,
                                          EndDate = c.EndDate,
                                          Organization = new BaseBriefModel()
                                          {
                                              Id = o.Id,
                                              Name = o.Name,
                                              NativeName = o.NativeName,
                                          },
                                          Event = new BaseBriefModel()
                                          {
                                              Id = e == null ? 0 : e.Id,
                                              Name = e == null ? "" : e.Name,
                                              NativeName = e == null ? "" : e.NativeName,
                                          }
                                      }).AsQueryable();
            return await selectionQueryable.Paginate(filters);
        }
        private async Task<IQueryable<Campaign>> ApplyAdvanceFilters(IQueryable<Campaign> campaignQueryable, CampaignSearchModel filters)
        {
            IQueryable<Campaign> filteredQueryable = campaignQueryable;
            FilteredRegionsModel filteredRegion;
            switch (filters.SearchType)
            {
                case RegionSearchTypeCatalog.InMyRegion:
                    filteredRegion = await GetRegionsByLatLong(filters);
                    break;
                case RegionSearchTypeCatalog.InRadius:
                    filteredRegion = await GetRegionsInRadius(filters);
                    break;
                case RegionSearchTypeCatalog.ByRegion:
                    filteredRegion = GetRegionsByRegionLevelAndId(filters.Regions);
                    break;
                default:
                    filteredRegion = null;
                    break;
            }
            if (filteredRegion != null)
            {
                filteredQueryable = (from c in campaignQueryable
                                     join er in _context.EntityRegions on c.Id equals er.EntityId
                                     where er.EntityType == (int)EntityRegionTypeCatalog.Campaign
                                     &&
                                     (
                                           (filteredRegion.Countries.Count == 0 || er.CountryId == null || filteredRegion.Countries.Select(x => x.Id).ToList().Contains(er.CountryId ?? 0))
                                        && (filteredRegion.States.Count == 0 || er.StateId == null || filteredRegion.States.Select(x => x.Id).ToList().Contains(er.StateId ?? 0))
                                        && (filteredRegion.Districts.Count == 0 || er.DistrictId == null || filteredRegion.Districts.Select(x => x.Id).ToList().Contains(er.DistrictId ?? 0))
                                        && (filteredRegion.Tehsils.Count == 0 || er.TehsilId == null || filteredRegion.Tehsils.Select(x => x.Id).ToList().Contains(er.TehsilId ?? 0))
                                        && (filteredRegion.UnionCouncils.Count == 0 || er.UnionCouncilId == null || filteredRegion.UnionCouncils.Select(x => x.Id).ToList().Contains(er.UnionCouncilId ?? 0))
                                      )
                                     select c
             ).Distinct().AsQueryable();
            }
            if (filters.RootCategories.Count() > 0)
            {
                filteredQueryable = (from c in filteredQueryable
                                     join oi in _context.OrganizationItems on c.Id equals oi.CampaignId
                                     join i in _context.Items on oi.ItemId equals i.Id
                                     where filters.RootCategories.Contains(i.RootId ?? 0)
                                     select c
                 ).Distinct().AsQueryable();

            }
            return filteredQueryable;
        }
    }
}
