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
        public async Task<PaginatedResultModel<OrganizationModel>> GetOrganizations(OrganizationSearchModel filters)
        {
            var organizationQueryable = (from o in _context.Organizations
                                         where
                                         (
                                           string.IsNullOrEmpty(filters.Name)
                                           || o.Name.Contains(filters.Name)
                                           || o.NativeName.Contains(filters.Name)
                                         )
                                         && o.IsDeleted == false
                                         select o).AsQueryable();
            var filteredQueryable = await ApplyAdvanceFilters(organizationQueryable, filters);
            var selectionQueryable = (from o in filteredQueryable
                                      join ob in _context.Members on o.OwnedBy equals ob.Id
                                      join po in _context.Organizations on o.ParentId equals po.Id into tpo
                                      from po in tpo.DefaultIfEmpty()
                                      select new OrganizationModel
                                      {
                                          Id = o.Id,
                                          Parent = new BaseBriefModel()
                                          {
                                              Id = po == null ? 0 : po.Id,
                                              Name = po == null ? "" : po.Name,
                                              NativeName = po == null ? "" : po.NativeName
                                          },
                                          Name = o.Name,
                                          NativeName = o.NativeName,
                                          Description = o.Description,
                                          ImageUrl = o.LogoUrl,
                                          Type = (OrganizationTypeCatalog)(o.Type ?? 0),
                                          OwnedBy = new MemberBriefModel()
                                          {
                                              Id = ob == null ? 0 : ob.Id,
                                              Name = ob == null ? "" : ob.Name,
                                              NativeName = ob == null ? "" : ob.NativeName
                                          },
                                          IsVerified = o.IsVerified,
                                          IsPeripheral = o.IsPeripheral,
                                          IsActive = o.IsActive,
                                          CreatedDate = o.CreatedDate,
                                      }).AsQueryable();
            return await selectionQueryable.Paginate(filters);
        }
        private async Task<IQueryable<Organization>> ApplyAdvanceFilters(IQueryable<Organization> organizationQueryable, OrganizationSearchModel filters)
        {
            IQueryable<Organization> filteredQueryable = organizationQueryable;
            FilteredRegionsModel filteredRegion = null;
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
                filteredQueryable = (from o in organizationQueryable
                                     join er in _context.EntityRegions on o.Id equals er.EntityId
                                     where er.EntityType == (int)EntityRegionTypeCatalog.Organization
                                     &&
                                     (
                                           (filteredRegion.Countries.Count == 0 || er.CountryId == null || filteredRegion.Countries.Select(x => x.Id).ToList().Contains(er.CountryId ?? 0))
                                        && (filteredRegion.States.Count == 0 || er.CountryId == null || filteredRegion.States.Select(x => x.Id).ToList().Contains(er.StateId ?? 0))
                                        && (filteredRegion.Districts.Count == 0 || er.CountryId == null || filteredRegion.Districts.Select(x => x.Id).ToList().Contains(er.DistrictId ?? 0))
                                        && (filteredRegion.Tehsils.Count == 0 || er.CountryId == null || filteredRegion.Tehsils.Select(x => x.Id).ToList().Contains(er.TehsilId ?? 0))
                                        && (filteredRegion.UnionCouncils.Count == 0 || er.CountryId == null || filteredRegion.UnionCouncils.Select(x => x.Id).ToList().Contains(er.UnionCouncilId ?? 0))
                                      //&& (filteredRegion.States.Count == 0 || er.StateId == null || filteredRegion.States.Any(x => x.Id == er.StateId))
                                      //&& (filteredRegion.Districts.Count == 0 || er.DistrictId == null || filteredRegion.Districts.Any(x => x.Id == er.DistrictId))
                                      //&& (filteredRegion.Tehsils.Count == 0 || er.TehsilId == null || filteredRegion.Tehsils.Any(x => x.Id == er.TehsilId))
                                      //&& (filteredRegion.UnionCouncils.Count == 0 || er.UnionCouncilId == null || filteredRegion.UnionCouncils.Any(x => x.Id == er.UnionCouncilId))
                                      )
                                     select o
                 ).Distinct().AsQueryable();
            }
            if (filters.RootCategories.Count() > 0)
            {
                filteredQueryable = (from o in filteredQueryable
                                     join oi in _context.OrganizationItems on o.Id equals oi.OrganizationId
                                     join i in _context.Items on oi.ItemId equals i.Id
                                     where filters.RootCategories.Contains(i.RootId ?? 0)
                                     select o
                 ).Distinct().AsQueryable();

            }
            return filteredQueryable;
        }

    }
}
