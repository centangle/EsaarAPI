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
        public async Task<bool> ModifyMultipleEntityRegion(List<EntityRegionModel> entityRegions)
        {
            using (CharityEntities context = new CharityEntities())
            {
                await ModifyMultipleEntityRegion(context, entityRegions);
                return true;
            }

        }
        private async Task<bool> ModifyMultipleEntityRegion(CharityEntities context, List<EntityRegionModel> entityRegions)
        {
            await ModifyEntityRegions(context, entityRegions);
            return await context.SaveChangesAsync() > 0;
        }
        private async Task<List<EntityRegion>> GetCurrentEntityRegions(CharityEntities context, EntityRegionModel entityRegion)
        {
            return await context.EntityRegions.Where(x => x.EntityId == entityRegion.Id && x.EntityType == (int)entityRegion.EntityType && x.IsDeleted == false).ToListAsync();
        }
        private EntityRegion SetEntityRegion(EntityRegion dbModel, EntityRegionModel model)
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
            return dbModel;

        }
        private void AddEntityRegions(CharityEntities context, ICollection<EntityRegionModel> entityRegions)
        {
            foreach (var item in entityRegions)
            {
                var dbModel = SetEntityRegion(new EntityRegion(), item);
                context.EntityRegions.Add(dbModel);
            }
        }
        private void UpdateEntityRegions(IEnumerable<EntityRegion> modfiedEntityRegions, IEnumerable<EntityRegionModel> entityRegion)
        {
            foreach (var dbModel in modfiedEntityRegions)
            {
                EntityRegionModel model = entityRegion.Where(x => x.Id == dbModel.Id).FirstOrDefault();
                SetEntityRegion(dbModel, model);
            }
        }
        private void DeleteEntityRegions(IEnumerable<EntityRegion> deletedEntityRegions)
        {
            foreach (var item in deletedEntityRegions)
            {
                item.IsDeleted = true;
            }
        }
        private async Task ModifyEntityRegions(CharityEntities context, List<EntityRegionModel> enityRegions)
        {
            var masterList = await GetCurrentEntityRegions(context, enityRegions.FirstOrDefault());
            var newItems = enityRegions.Where(x => x.Id == 0).ToList();
            var updatedItems = masterList.Where(m => enityRegions.Any(s => m.Id == s.Id));
            var deletedItems = masterList.Where(m => !enityRegions.Any(s => m.Id == s.Id));
            AddEntityRegions(context, newItems);
            UpdateEntityRegions(updatedItems, enityRegions);
            DeleteEntityRegions(deletedItems);
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
