using AutoMapper;
using Catalogs;
using Dapper;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateOrganization(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetOrganization(new Organization(), model, true);
                if (model.ParentId != 0)
                    dbModel.ParentId = model.ParentId;
                context.Organizations.Add(dbModel);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        public async Task<bool> UpdateOrganization(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Organization dbModel = await context.Organizations.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetOrganization(dbModel, model, false);
                    if (model.ParentId != 0)
                        dbModel.ParentId = model.ParentId;
                    else
                        dbModel.ParentId = null;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<int> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var dbModel = AddTreeNode<Organization, OrganizationModel>(model, true);
                        context.Organizations.Add(dbModel);
                        await context.SaveChangesAsync();
                        model.Id = dbModel.Id;
                        transaction.Commit();
                        return model.Id;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }
        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var root = (await GetSingleItemTree<Organization, Organization>(context, model.Id, false)).First();
                var OrganizationToUpdate = context.Organizations.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefault();
                if (OrganizationToUpdate != null)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            UpdateTreeNode(context, root, model);
                            await context.SaveChangesAsync();
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
                return false;
            }
        }
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var Organization in Organizations)
                        {
                            var dbModel = AddTreeNode<Organization, OrganizationModel>(Organization, true);
                            context.Organizations.Add(dbModel);
                            await context.SaveChangesAsync();
                            Organization.Id = dbModel.Id;
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> Organizations)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var Organization in Organizations)
                        {
                            var root = (await GetSingleOrganizationTree<Organization, Organization>(context, Organization.Id, false)).First();
                            var OrganizationToUpdate = context.Organizations.Where(x => x.Id == Organization.Id && x.IsDeleted == false).FirstOrDefault();
                            if (OrganizationToUpdate != null)
                            {
                                UpdateTreeNode(context, root, Organization);
                                await context.SaveChangesAsync();
                            }
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        public async Task<bool> DeleteOrganization(int Id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Organization dbModel = await context.Organizations.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var root = (await GetSingleOrganizationTree<Organization, Organization>(context, dbModel.Id, false)).First();
                    DeleteItemNode(root);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        private Organization SetOrganization(Organization dbModel, OrganizationModel model, bool isNew)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Description = model.Description;
            ImageHelper.Save(model);
            dbModel.LogoUrl = model.ImageUrl;
            if (model.OwnedBy == null)
            {
                throw new KnownException("Organization must have an owner.");
            }
            else
                dbModel.OwnedBy = model.OwnedBy.Id;
            dbModel.IsVerified = model.IsVerified;
            dbModel.IsPeripheralOrganization = model.IsPeripheralOrganization;
            dbModel.IsActive = model.IsActive;
            if (isNew)
            {
                dbModel.IsDeleted = false;
                dbModel.CreatedDate = DateTime.UtcNow;
            }
            else
                dbModel.IsDeleted = model.IsDeleted;
            return dbModel;

        }
        public async Task<OrganizationModel> GetOrganization(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from o in context.Organizations
                              join ob in context.People on o.OwnedBy equals ob.Id
                              join po in context.Organizations on o.ParentId equals po.Id into tpo
                              from po in tpo.DefaultIfEmpty()
                              where o.Id == id
                              && o.IsDeleted == false
                              select new OrganizationModel
                              {
                                  Id = o.Id,
                                  Parent = new BriefModel()
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
                                  OwnedBy = new BriefModel()
                                  {
                                      Id = ob == null ? 0 : ob.Id,
                                      Name = ob == null ? "" : ob.Name,
                                      NativeName = ob == null ? "" : ob.NativeName
                                  },
                                  IsVerified = o.IsVerified,
                                  IsPeripheralOrganization = o.IsPeripheralOrganization,
                                  IsActive = o.IsActive,
                                  CreatedDate = o.CreatedDate,
                              }).FirstOrDefaultAsync();
            }
        }

        public async Task<List<OrganizationModel>> GetPeripheralOrganizations()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from o in context.Organizations
                              join ob in context.People on o.OwnedBy equals ob.Id
                              join po in context.Organizations on o.ParentId equals po.Id into tpo
                              from po in tpo.DefaultIfEmpty()
                              where o.IsPeripheralOrganization == true
                              && o.IsDeleted == false
                              select new OrganizationModel
                              {
                                  Id = o.Id,
                                  Parent = new BriefModel()
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
                                  OwnedBy = new BriefModel()
                                  {
                                      Id = ob == null ? 0 : ob.Id,
                                      Name = ob == null ? "" : ob.Name,
                                      NativeName = ob == null ? "" : ob.NativeName
                                  },
                                  IsVerified = o.IsVerified,
                                  IsPeripheralOrganization = o.IsPeripheralOrganization,
                                  IsActive = o.IsActive,
                                  CreatedDate = o.CreatedDate,
                              }).ToListAsync();
            }
        }

        public async Task<List<OrganizationModel>> GetRootOrganizations()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from o in context.Organizations
                              join ob in context.People on o.OwnedBy equals ob.Id
                              join po in context.Organizations on o.ParentId equals po.Id into tpo
                              from po in tpo.DefaultIfEmpty()
                              where o.ParentId == null
                              && o.IsDeleted == false
                              select new OrganizationModel
                              {
                                  Id = o.Id,
                                  Parent = new BriefModel()
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
                                  OwnedBy = new BriefModel()
                                  {
                                      Id = ob == null ? 0 : ob.Id,
                                      Name = ob == null ? "" : ob.Name,
                                      NativeName = ob == null ? "" : ob.NativeName
                                  },
                                  IsVerified = o.IsVerified,
                                  IsPeripheralOrganization = o.IsPeripheralOrganization,
                                  IsActive = o.IsActive,
                                  CreatedDate = o.CreatedDate,
                              }).ToListAsync();
            }
        }

        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(bool getHierarchicalData)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var OrganizationsDBList = await context.Organizations.SqlQuery(GetAllOrganizationsTreeQuery()).AsNoTracking().ToListAsync();
                MapperConfiguration mapperConfig = GetOrganizationMapperConfig();
                return GetAllNodes<OrganizationModel, Organization, OrganizationModel>(mapperConfig, OrganizationsDBList, true, getHierarchicalData);
            }

        }
        public async Task<IEnumerable<OrganizationModel>> GetSingleTreeOrganization(int id, bool getHierarchicalData)
        {
            using (CharityEntities context = new CharityEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return await GetSingleOrganizationTree<OrganizationModel, OrganizationModel>(context, id, true, getHierarchicalData);
            }
        }
        private async Task<IEnumerable<T>> GetSingleOrganizationTree<T, M>(CharityEntities context, int id, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where M : class, ITree<M>
        {

            context.Configuration.AutoDetectChangesEnabled = false;
            var OrganizationsDBList = await context.Organizations.SqlQuery(GetOrganizationTreeQuery(), new SqlParameter("@Id", id)).ToListAsync();
            MapperConfiguration mapperConfig = GetOrganizationMapperConfig();
            var Organizations = GetSingleNodeTree<T, Organization, M>(id, mapperConfig, OrganizationsDBList, returnViewModel, getHierarchicalData);
            context.Configuration.AutoDetectChangesEnabled = true;
            return Organizations;
        }

        private string GetAllOrganizationsTreeQuery()
        {
            return @"
                        WITH cte As
                        (
                            SELECT ParentOrganization.*
                            FROM Organization ParentOrganization
                            WHERE ParentId is null
                            and IsDeleted=0
                            UNION All
                                SELECT ChildOrganization.*
                                FROM Organization ChildOrganization
                                JOIN cte
                                On cte.id = ChildOrganization.ParentId
                                where ChildOrganization.IsDeleted=0
                        )
                        SELECT*
                        FROM cte";
        }
        private string GetOrganizationTreeQuery()
        {
            return @"
                        WITH cte As
                        (
                            SELECT ParentOrganization.*
                            FROM Organization ParentOrganization
                            WHERE Id = @Id
                            and IsDeleted=0
                            UNION All
                                SELECT ChildOrganization.*
                                FROM Organization ChildOrganization
                                JOIN cte
                                On cte.id = ChildOrganization.ParentId
                                where ChildOrganization.IsDeleted=0
                        )
                        SELECT*
                        FROM cte";
        }
        private MapperConfiguration GetOrganizationMapperConfig()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Organization, OrganizationModel>()
               .ForMember(dest => dest.Parent,
               input => input.MapFrom(i => new BriefModel { Id = i.ParentId ?? 0 }))
               .ForMember(s => s.Children, m => m.Ignore())
               );

        }
    }
}
