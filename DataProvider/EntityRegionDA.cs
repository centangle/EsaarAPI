using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<PaginatedResultModel<PaginatedEntityRegionModel>> GetEntityRegions(EntityRegionSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var entityRegionQueryable = (from er in context.EntityRegions
                                             where er.EntityId == filters.EntityId
                                             && er.EntityType == (int)filters.EntityType
                                             && er.IsDeleted == false
                                             select new PaginatedEntityRegionModel
                                             {
                                                 Id = er.Id,
                                                 Entity = new BaseBriefModel()
                                                 {
                                                     Id = er.Id,
                                                 },
                                                 EntityType = (EntityRegionTypeCatalog)er.EntityType,
                                                 Region = new RegionBriefModel()
                                                 {
                                                     Id = er.Id,
                                                 },
                                                 RegionLevel = (RegionLevelTypeCatalog)er.RegionLevel,
                                                 IsApproved = er.IsApproved,
                                                 IsActive = er.IsActive,
                                                 CreatedDate = er.CreatedDate,

                                             }).AsQueryable();
                var paginatedResult = await entityRegionQueryable.Paginate(filters);
                if (paginatedResult != null & paginatedResult.TotalCount > 0)
                {
                    var entity = await GetEntityRegionEntity(context, filters.EntityId, filters.EntityType);
                    foreach (var entityRegion in paginatedResult.Items)
                    {
                        entityRegion.Entity = entity;
                        entityRegion.Region = await GetEntityRegionRegion(context, entityRegion.Region.Id, entityRegion.RegionLevel);
                    }
                }
                return paginatedResult;
            }
        }
        public async Task<bool> ModifyMultipleEntityRegion(List<EntityRegionModel> entityRegions, int organizationId, int? requestId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                await ModifyMultipleEntityRegion(context, entityRegions, organizationId, requestId);
                return true;
            }
        }
        private async Task<bool> ModifyMultipleEntityRegion(CharityEntities context, List<EntityRegionModel> entityRegions, int organizationId, int? requestId)
        {
            await ModifyEntityRegions(context, entityRegions, organizationId, requestId);
            return await context.SaveChangesAsync() > 0;
        }
        private async Task<List<EntityRegion>> GetCurrentEntityRegions(CharityEntities context, EntityRegionModel entityRegion, int? requestId)
        {
            var queryableRegions = context.EntityRegions.Where(x => x.EntityId == entityRegion.Entity.Id
            && x.EntityType == (int)entityRegion.EntityType
            && x.IsDeleted == false).AsQueryable();
            if (requestId != null)
            {
                queryableRegions.Where(x => x.RequestId == entityRegion.RequestId);
            }
            return await queryableRegions.ToListAsync();
        }
        private async Task<EntityRegion> SetEntityRegion(CharityEntities context, EntityRegion dbModel, EntityRegionModel model)
        {
            if (model.Entity == null || model.Entity.Id == 0)
            {
                throw new KnownException("Entity is required");
            }
            dbModel.EntityId = model.Entity.Id;
            dbModel.EntityType = (int)model.EntityType;
            if (model.Region == null || model.Region.Id == 0)
            {
                throw new KnownException("Region is required");
            }
            dbModel.RegionId = model.Region.Id;
            dbModel.RegionLevel = (int)model.RegionLevel;
            dbModel.IsApproved = model.IsApproved;
            if (dbModel.IsApproved == true)
            {
                dbModel.ApprovedBy = _loggedInMemberId;
            }
            SetAndValidateBaseProperties(dbModel, model);
            await SetAllParentRegionLevel(context, dbModel, model);
            return dbModel;

        }
        private async Task SetAllParentRegionLevel(CharityEntities context, EntityRegion dbModel, EntityRegionModel model)
        {
            RegionBriefModel country = null;
            RegionBriefModel state = null;
            RegionBriefModel district = null;
            RegionBriefModel tehsil = null;
            RegionBriefModel uc = null;
            if (model.RegionLevel == RegionLevelTypeCatalog.UnionCouncil)
            {
                uc = await GetUnionCouncil(context, model.Region.Id);
                tehsil = await GetTehsil(context, uc.ParentId);
                district = await GetDistrict(context, tehsil.ParentId);
                state = await GetState(context, district.ParentId);
                country = await GetCountry(context, state.ParentId);
            }
            else if (model.RegionLevel == RegionLevelTypeCatalog.Tehsil)
            {
                tehsil = await GetTehsil(context, model.Region.Id);
                district = await GetDistrict(context, tehsil.ParentId);
                state = await GetState(context, district.ParentId);
                country = await GetCountry(context, state.ParentId);
            }
            else if (model.RegionLevel == RegionLevelTypeCatalog.District)
            {
                district = await GetDistrict(context, model.Region.Id);
                state = await GetState(context, district.ParentId);
                country = await GetCountry(context, state.ParentId);
            }
            else if (model.RegionLevel == RegionLevelTypeCatalog.State)
            {
                state = await GetState(context, model.Region.Id);
                country = await GetCountry(context, state.ParentId);
            }
            else
            {
                country = await GetCountry(context, model.Region.Id);
            }
            dbModel.CountryId = country?.Id;
            dbModel.StateId = state?.Id;
            dbModel.DistrictId = district?.Id;
            dbModel.TehsilId = tehsil?.Id;
            dbModel.UnionCouncilId = uc?.Id;
        }
        private async Task AddEntityRegions(CharityEntities context, ICollection<EntityRegionModel> entityRegions)
        {
            foreach (var item in entityRegions)
            {
                var dbModel = await SetEntityRegion(context, new EntityRegion(), item);
                context.EntityRegions.Add(dbModel);
            }
        }
        private async Task UpdateEntityRegions(CharityEntities context, IEnumerable<EntityRegion> modfiedEntityRegions, IEnumerable<EntityRegionModel> entityRegion)
        {
            foreach (var dbModel in modfiedEntityRegions)
            {
                EntityRegionModel model = entityRegion.Where(x => x.Id == dbModel.Id).FirstOrDefault();
                await SetEntityRegion(context, dbModel, model);
            }
        }
        private void DeleteEntityRegions(IEnumerable<EntityRegion> deletedEntityRegions)
        {
            foreach (var item in deletedEntityRegions)
            {
                item.IsDeleted = true;
            }
        }
        private async Task ModifyEntityRegions(CharityEntities context, List<EntityRegionModel> enityRegions, int organizationId, int? requestId, bool skipModeratorCheck = false)
        {
            var organizationMember = await GetMemberRoleForOrganization(context, organizationId, _loggedInMemberId);
            if (IsOrganizationMemberModerator(organizationMember.FirstOrDefault()) || skipModeratorCheck)
            {
                var masterList = await GetCurrentEntityRegions(context, enityRegions.FirstOrDefault(), requestId);
                var newItems = enityRegions.Where(x => x.Id == 0).ToList();
                var updatedItems = masterList.Where(m => enityRegions.Any(s => m.Id == s.Id));
                var deletedItems = masterList.Where(m => !enityRegions.Any(s => m.Id == s.Id));
                await AddEntityRegions(context, newItems);
                await UpdateEntityRegions(context, updatedItems, enityRegions);
                DeleteEntityRegions(deletedItems);
            }
            else
            {
                throw new KnownException("You are not authorized to perform this action.");
            }
        }
        private async Task<BaseBriefModel> GetEntityRegionEntity(CharityEntities context, int entityId, EntityRegionTypeCatalog entityType)
        {
            if (entityType == EntityRegionTypeCatalog.Organization)
            {
                return await (from o in context.Organizations
                              where o.Id == entityId
                              && o.IsDeleted == false
                              select new BaseBriefModel
                              {
                                  Id = o.Id,
                                  Name = o.Name,
                                  NativeName = o.NativeName,
                              }).FirstOrDefaultAsync();
            }
            else if (entityType == EntityRegionTypeCatalog.OrganizationMember)
            {
                return await (from om in context.OrganizationMembers
                              join m in context.Members on om.MemberId equals m.Id
                              where m.Id == entityId
                              && m.IsDeleted == false
                              select new BaseBriefModel
                              {
                                  Id = m.Id,
                                  Name = m.Name,
                                  NativeName = m.NativeName,
                              }).FirstOrDefaultAsync();
            }
            return null;
        }
        private async Task<RegionBriefModel> GetEntityRegionRegion(CharityEntities context, int regionId, RegionLevelTypeCatalog regionLevel)
        {
            switch (regionLevel)
            {
                case RegionLevelTypeCatalog.Country:
                    return await GetCountry(context, regionId);
                case RegionLevelTypeCatalog.State:
                    return await GetState(context, regionId);
                case RegionLevelTypeCatalog.District:
                    return await GetDistrict(context, regionId);
                case RegionLevelTypeCatalog.Tehsil:
                    return await GetTehsil(context, regionId);
                case RegionLevelTypeCatalog.UnionCouncil:
                    return await GetUnionCouncil(context, regionId);
            }
            return null;
        }
    }
}
