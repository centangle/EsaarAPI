using AutoMapper;
using Catalogs;
using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateOrganization(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var dbModel = SetOrganization(new Organization(), model);
                        if (model.ParentId != 0)
                            dbModel.ParentId = model.ParentId;
                        context.Organizations.Add(dbModel);
                        await context.SaveChangesAsync();
                        model.Id = dbModel.Id;
                        OrganizationMemberModel membershipModel = new OrganizationMemberModel
                        {
                            Organization = new BaseBriefModel
                            {
                                Id = model.Id,
                            },
                            Member = new BaseBriefModel()
                            {
                                Id = model.OwnedBy.Id,
                            },
                            Role = OrganizationMemberRolesCatalog.Owner
                        };

                        await AddMemberToOrganization(context, membershipModel);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                        return model.Id;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> UpdateOrganization(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Organization dbModel = await context.Organizations.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetOrganization(dbModel, model);
                    if (model.ParentId != 0)
                        dbModel.ParentId = model.ParentId;
                    else
                        dbModel.ParentId = null;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<bool> CreateSingleOrganizationWithChildrens(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentDbNodes = new List<Organization>();
                var allNodes = TreeHelper.TreeToList(new List<OrganizationModel> { model });
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Organizations, currentDbNodes, allNodes);
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
        public async Task<bool> UpdateSingleOrganizationWithChildren(OrganizationModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var allNodes = TreeHelper.TreeToList(new List<OrganizationModel> { model });
                var currentRootNode = allNodes.Where(x => x.Node.ParentId == null || x.Node.ParentId == 0).Select(x => x.Node.Id).FirstOrDefault();
                var currentDbNodes = (await GetSingleOrganizationTree<Organization, Organization>(context, currentRootNode, false, false)).ToList();

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Organizations, currentDbNodes, allNodes);
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
        public async Task<bool> CreateMultipleOrganizationsWithChildrens(List<OrganizationModel> organizations)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentDbNodes = new List<Organization>();
                var allNodes = TreeHelper.TreeToList(organizations);
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Organizations, currentDbNodes, allNodes);
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
        public async Task<bool> UpdateMultipleOrganizationsWithChildrens(List<OrganizationModel> organizations)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentDbNodes = (await GetAllOrganizations<Organization, Organization>(context, false, false)).ToList();
                var allNodes = TreeHelper.TreeToList(organizations);
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Organizations, currentDbNodes, allNodes);
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
                Organization dbModel = await context.Organizations.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var root = (await GetSingleOrganizationTree<Organization, Organization>(context, dbModel.Id, false)).First();
                    DeleteTreeNode(root);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        private Organization SetOrganization(Organization dbModel, OrganizationModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Description = model.Description;
            ImageHelper.Save(model);
            dbModel.LogoUrl = model.ImageUrl;
            model.OwnedBy = SetEntityId(model.OwnedBy, "Organization must have an owner.");
            dbModel.OwnedBy = model.OwnedBy.Id;
            dbModel.IsVerified = model.IsVerified;
            dbModel.IsPeripheral = model.IsPeripheral;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;

        }
        public async Task<OrganizationModel> GetOrganization(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                List<OrganizationMemberRolesCatalog> currentMemberRoles = new List<OrganizationMemberRolesCatalog>();
                var memberOrgRoles = (await GetMemberRoleForOrganization(id, _loggedInMemberId)).FirstOrDefault();
                if (memberOrgRoles != null)
                {
                    currentMemberRoles = memberOrgRoles.Roles;
                }
                return await (from o in context.Organizations
                              join ob in context.Members on o.OwnedBy equals ob.Id
                              join po in context.Organizations on o.ParentId equals po.Id into tpo
                              from po in tpo.DefaultIfEmpty()
                              where o.Id == id
                              && o.IsDeleted == false
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
                                  CurrentMemberRoles = currentMemberRoles,
                                  IsVerified = o.IsVerified,
                                  IsPeripheral = o.IsPeripheral,
                                  IsActive = o.IsActive,
                                  CreatedDate = o.CreatedDate,
                              }).FirstOrDefaultAsync();
            }
        }
        public async Task<PaginatedResultModel<OrganizationModel>> GetOrganizations(OrganizationSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {

                var organizationQueryable = (from o in context.Organizations
                                             join ob in context.Members on o.OwnedBy equals ob.Id
                                             join po in context.Organizations on o.ParentId equals po.Id into tpo
                                             from po in tpo.DefaultIfEmpty()
                                             where
                                             (
                                               string.IsNullOrEmpty(filters.Name)
                                               || o.Name.Contains(filters.Name)
                                               || o.NativeName.Contains(filters.Name)
                                             )
                                             && o.IsDeleted == false
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
                return await organizationQueryable.Paginate(filters);
            }
        }
        public async Task<List<OrganizationModel>> GetPeripheralOrganizations()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from o in context.Organizations
                              join ob in context.Members on o.OwnedBy equals ob.Id
                              join po in context.Organizations on o.ParentId equals po.Id into tpo
                              from po in tpo.DefaultIfEmpty()
                              where o.IsPeripheral == true
                              && o.IsDeleted == false
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
                              }).ToListAsync();
            }
        }
        public async Task<List<OrganizationModel>> GetRootOrganizations()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from o in context.Organizations
                              join ob in context.Members on o.OwnedBy equals ob.Id
                              join po in context.Organizations on o.ParentId equals po.Id into tpo
                              from po in tpo.DefaultIfEmpty()
                              where o.ParentId == null
                              && o.IsDeleted == false
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
                              }).ToListAsync();
            }
        }
        public async Task<IEnumerable<OrganizationModel>> GetAllOrganizations(bool getHierarchicalData)
        {
            using (CharityEntities context = new CharityEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return await GetAllOrganizations<OrganizationModel, OrganizationModel>(context, true, getHierarchicalData);
            }
        }
        private async Task<IEnumerable<T>> GetAllOrganizations<T, M>(CharityEntities context, bool returnViewModel, bool getHierarchicalData)
            where T : class, IBase
            where M : class, ITree<M>
        {
            var organizationsDBList = await context.Organizations.SqlQuery(GetAllOrganizationsTreeQuery()).ToListAsync();
            MapperConfiguration mapperConfig = GetOrganizationMapperConfig();
            return GetAllNodes<T, Organization, M>(mapperConfig, organizationsDBList, returnViewModel, getHierarchicalData);

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
               .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.LogoUrl))
               .ForMember(dest => dest.Parent,
               input => input.MapFrom(i => new BaseBriefModel { Id = i.ParentId ?? 0 }))
               .ForMember(dest => dest.OwnedBy,
               input => input.MapFrom(i => new MemberBriefModel { Id = i.OwnedBy }))
               .ForMember(s => s.children, m => m.Ignore())
               );

        }
    }
}
