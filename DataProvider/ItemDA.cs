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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateItem(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetItem(new Item(), model);
                if (model.ParentId != 0)
                    dbModel.ParentId = model.ParentId;
                if (model.RootId != 0)
                    dbModel.RootId = model.RootId;
                context.Items.Add(dbModel);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        public async Task<bool> UpdateItem(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Item dbModel = await context.Items.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetItem(dbModel, model);
                    if (model.ParentId != 0)
                        dbModel.ParentId = model.ParentId;
                    else
                        dbModel.ParentId = null;
                    if (model.RootId != 0)
                        dbModel.RootId = model.RootId;
                    else
                        dbModel.RootId = null;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<bool> CreateSingleItemWithChildrens(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentDbNodes = new List<Item>();
                var allNodes = TreeHelper.TreeToList(new List<ItemModel> { model });
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Items, currentDbNodes, allNodes);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {

                var allNodes = TreeHelper.TreeToList(new List<ItemModel> { model });
                var currentRootNode = allNodes.Where(x => x.Node.ParentId == null || x.Node.ParentId == 0).Select(x => x.Node.Id).FirstOrDefault();
                var currentDbNodes = (await GetSingleItemTree<Item, Item>(context, currentRootNode, false, false)).ToList();

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Items, currentDbNodes, allNodes);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentDbNodes = new List<Item>();
                var allNodes = TreeHelper.TreeToList(items);
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Items, currentDbNodes, allNodes);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var currentDbNodes = (await GetAllItems<Item, Item>(context, false, false)).ToList();
                var allNodes = TreeHelper.TreeToList(items);
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        await ModifyTreeNodes(context, context.Items, currentDbNodes, allNodes);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> DeleteItem(int id, int? organizationId = null)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var itemQueryable = context.Items.Where(x => x.Id == id && x.IsDeleted == false);
                if (organizationId != null)
                    itemQueryable.Where(x => x.OrganizationId == organizationId);
                Item dbModel = await itemQueryable.FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var root = (await GetSingleItemTree<Item, Item>(context, dbModel.Id, false)).First();
                    DeleteTreeNode(root);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        private Item SetItem(Item dbModel, ItemBaseModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            if (model.IsPeripheral)
            {
                if (model.DefaultUOM == null || model.DefaultUOM.Id == 0)
                {
                    throw new KnownException("Peripheral Items UOM must be set.");
                }
            }
            //Set UOM
            if (model.DefaultUOM == null || model.DefaultUOM.Id == 0)
                dbModel.DefaultUOM = null;
            else
                dbModel.DefaultUOM = model.DefaultUOM.Id;
            //Set Organization
            if (model.Organization == null || model.Organization.Id == 0)
                dbModel.OrganizationId = null;
            else
                dbModel.OrganizationId = model.Organization.Id;
            //Set Root
            if (model.Root == null || model.Root.Id == 0)
                dbModel.RootId = null;
            else
                dbModel.RootId = model.Root.Id;
            dbModel.Type = (int)model.Type;
            dbModel.Description = model.Description;
            dbModel.IsPeripheral = model.IsPeripheral;
            SetAndValidateBaseProperties(dbModel, model);
            ImageHelper.Save(model);
            dbModel.ImageUrl = model.ImageUrl;
            return dbModel;

        }
        public async Task<ItemModel> GetItem(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from i in context.Items
                              join pi in context.Items on i.ParentId equals pi.Id into tpi
                              from pi in tpi.DefaultIfEmpty()
                              join uom in context.UOMs on i.DefaultUOM equals uom.Id into tuom
                              from uom in tuom.DefaultIfEmpty()
                              where i.Id == id
                              && i.OrganizationId == null
                              && i.IsDeleted == false
                              select new ItemModel
                              {
                                  Id = i.Id,
                                  Parent = new BaseBriefModel()
                                  {
                                      Id = pi == null ? 0 : pi.Id,
                                      Name = pi == null ? "" : pi.Name,
                                      NativeName = pi == null ? "" : pi.NativeName
                                  },
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  DefaultUOM = new UOMBriefModel()
                                  {
                                      Id = uom == null ? 0 : uom.Id,
                                      Name = uom == null ? "" : uom.Name,
                                      NativeName = uom == null ? "" : uom.NativeName,
                                      NoOfBaseUnit = uom == null ? 0 : uom.NoOfBaseUnit,
                                  },
                                  Type = (ItemTypeCatalog)(i.Type ?? 0),
                                  Description = i.Description,
                                  ImageUrl = i.ImageUrl,
                                  IsPeripheral = i.IsPeripheral,
                                  IsActive = i.IsActive,
                              }).FirstOrDefaultAsync();
            }
        }
        public async Task<List<ItemModel>> GetPeripheralItems(int? organizationId = null)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from i in context.Items
                              join pi in context.Items on i.ParentId equals pi.Id into tpi
                              from pi in tpi.DefaultIfEmpty()
                              join uom in context.UOMs on i.DefaultUOM equals uom.Id into tuom
                              from uom in tuom.DefaultIfEmpty()
                              where i.IsPeripheral == true
                              && (i.OrganizationId == null || i.OrganizationId == organizationId)
                              && i.IsDeleted == false
                              select new ItemModel
                              {
                                  Id = i.Id,
                                  Parent = new BaseBriefModel()
                                  {
                                      Id = pi == null ? 0 : pi.Id,
                                      Name = pi == null ? "" : pi.Name,
                                      NativeName = pi == null ? "" : pi.NativeName
                                  },
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  DefaultUOM = new UOMBriefModel()
                                  {
                                      Id = uom == null ? 0 : uom.Id,
                                      Name = uom == null ? "" : uom.Name,
                                      NativeName = uom == null ? "" : uom.NativeName,
                                      NoOfBaseUnit = uom == null ? 0 : uom.NoOfBaseUnit,
                                  },
                                  Type = (ItemTypeCatalog)(i.Type??0),
                                  Description = i.Description,
                                  ImageUrl = i.ImageUrl,
                                  IsPeripheral = i.IsPeripheral,
                                  IsActive = i.IsActive,
                              }).ToListAsync();
            }
        }
        public async Task<List<ItemModel>> GetRootItems()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from i in context.Items
                              join pi in context.Items on i.ParentId equals pi.Id into tpi
                              from pi in tpi.DefaultIfEmpty()
                              join uom in context.UOMs on i.DefaultUOM equals uom.Id into tuom
                              from uom in tuom.DefaultIfEmpty()
                              where i.ParentId == null
                              && i.IsDeleted == false
                              && i.OrganizationId == null
                              select new ItemModel
                              {
                                  Id = i.Id,
                                  Parent = new BaseBriefModel()
                                  {
                                      Id = pi == null ? 0 : pi.Id,
                                      Name = pi == null ? "" : pi.Name,
                                      NativeName = pi == null ? "" : pi.NativeName
                                  },
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  DefaultUOM = new UOMBriefModel()
                                  {
                                      Id = uom == null ? 0 : uom.Id,
                                      Name = uom == null ? "" : uom.Name,
                                      NativeName = uom == null ? "" : uom.NativeName,
                                      NoOfBaseUnit = uom == null ? 0 : uom.NoOfBaseUnit,
                                  },
                                  Type = (ItemTypeCatalog)(i.Type ?? 0),
                                  Description = i.Description,
                                  ImageUrl = i.ImageUrl,
                                  IsPeripheral = i.IsPeripheral,
                                  IsActive = i.IsActive,
                              }).ToListAsync();
            }
        }
        public async Task<IEnumerable<ItemModel>> GetAllItems(bool getHierarchicalData)
        {
            using (CharityEntities context = new CharityEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return await GetAllItems<ItemModel, ItemModel>(context, true, getHierarchicalData);
            }
        }
        private async Task<IEnumerable<T>> GetAllItems<T, M>(CharityEntities context, bool returnViewModel, bool getHierarchicalData)
            where T : class, IBase
            where M : class, ITree<M>
        {
            var itemsDBList = await context.Items.SqlQuery(GetAllItemsTreeQuery()).ToListAsync();
            MapperConfiguration mapperConfig = GetItemMapperConfig();
            return GetAllNodes<T, Item, M>(mapperConfig, itemsDBList, returnViewModel, getHierarchicalData);

        }
        public async Task<IEnumerable<ItemModel>> GetSingleTreeItem(int id, bool getHierarchicalData)
        {
            using (CharityEntities context = new CharityEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                return await GetSingleItemTree<ItemModel, ItemModel>(context, id, true, getHierarchicalData);
            }
        }
        private async Task<IEnumerable<T>> GetSingleItemTree<T, M>(CharityEntities context, int id, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where M : class, ITree<M>
        {

            context.Configuration.AutoDetectChangesEnabled = false;
            var ItemsDBList = await context.Items.SqlQuery(GetItemTreeQuery(), new SqlParameter("@Id", id)).ToListAsync();
            MapperConfiguration mapperConfig = GetItemMapperConfig();
            var items = GetSingleNodeTree<T, Item, M>(id, mapperConfig, ItemsDBList, returnViewModel, getHierarchicalData);
            context.Configuration.AutoDetectChangesEnabled = true;
            return items;
        }
        private string GetAllItemsTreeQuery()
        {
            return @"
                        WITH cte As
                        (
                            SELECT ParentItem.*
                            FROM Item ParentItem
                            WHERE ParentId is null
                            and IsDeleted=0
                            and OrganizationId is null
                            UNION All
                                SELECT ChildItem.*
                                FROM Item ChildItem
                                JOIN cte
                                On cte.id = ChildItem.ParentId
                                where ChildItem.IsDeleted=0
                        )
                        SELECT*
                        FROM cte";
        }
        private string GetItemTreeQuery()
        {
            return @"
                        WITH cte As
                        (
                            SELECT ParentItem.*
                            FROM Item ParentItem
                            WHERE Id = @Id
                            and IsDeleted=0
                            and OrganizationId is null
                            UNION All
                                SELECT ChildItem.*
                                FROM Item ChildItem
                                JOIN cte
                                On cte.id = ChildItem.ParentId
                                where ChildItem.IsDeleted=0
                        )
                        SELECT*
                        FROM cte";
        }
        private MapperConfiguration GetItemMapperConfig()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Item, ItemModel>()
               .ForMember(dest => dest.DefaultUOM,
               input => input.MapFrom(i => new BaseBriefModel { Id = i.DefaultUOM ?? 0 }))
               .ForMember(dest => dest.Parent,
               input => input.MapFrom(i => new BaseBriefModel { Id = i.ParentId ?? 0 }))
               .ForMember(s => s.children, m => m.Ignore())
               );

        }
    }
}
