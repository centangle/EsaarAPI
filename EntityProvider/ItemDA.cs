using AutoMapper;
using Catalogs;
using EntityProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateItem(ItemModel model)
        {
            var dbModel = SetItem(new Item(), model);
            if (model.ParentId != 0)
                dbModel.ParentId = model.ParentId;
            if (model.RootId != 0)
                dbModel.RootId = model.RootId;
            _context.Items.Add(dbModel);
            await _context.SaveChangesAsync();
            model.Id = dbModel.Id;
            return model.Id;
        }
        public async Task<bool> UpdateItem(ItemModel model)
        {
            Item dbModel = await _context.Items.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
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
                return await _context.SaveChangesAsync() > 0;
            }
            return false;

        }
        public async Task<bool> CreateSingleItemWithChildrens(ItemModel model)
        {
            var currentDbNodes = new List<Item>();
            var allNodes = TreeHelper.TreeToList(new List<ItemModel> { model });
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Items, currentDbNodes, allNodes);
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
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            var allNodes = TreeHelper.TreeToList(new List<ItemModel> { model });
            var currentRootNode = allNodes.Where(x => x.Node.ParentId == null || x.Node.ParentId == 0).Select(x => x.Node.Id).FirstOrDefault();
            var currentDbNodes = (await GetSingleItemTree<Item, Item>(_context, currentRootNode, false, false)).ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Items, currentDbNodes, allNodes);
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
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            var currentDbNodes = new List<Item>();
            var allNodes = TreeHelper.TreeToList(items);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Items, currentDbNodes, allNodes);
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
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            var currentDbNodes = (await GetAllItems<Item, Item>(_context, false, false)).ToList();
            var allNodes = TreeHelper.TreeToList(items);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Items, currentDbNodes, allNodes);
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
        public async Task<bool> DeleteItem(int id, int? organizationId = null)
        {
            var itemQueryable = _context.Items.Where(x => x.Id == id && x.IsDeleted == false);
            if (organizationId != null)
                itemQueryable.Where(x => x.OrganizationId == organizationId);
            Item dbModel = await itemQueryable.FirstOrDefaultAsync();
            if (dbModel != null)
            {
                var root = (await GetSingleItemTree<Item, Item>(_context, dbModel.Id, false)).First();
                Dictionary<int, string> deletedItems = new Dictionary<int, string>();
                DeleteTreeNode(root, deletedItems);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
        private Item SetItem(Item dbModel, ItemBaseModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            if (model.IsPeripheral)
            {
                if (model.DefaultUOM == null || model.DefaultUOM.Id == 0)
                {
                    throw new KnownException("Peripheral Items Uom must be set.");
                }
            }
            //Set Uom
            if (model.DefaultUOM == null || model.DefaultUOM.Id == 0)
                dbModel.DefaultUom = null;
            else
                dbModel.DefaultUom = model.DefaultUOM.Id;
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
            dbModel.Worth = model.Worth;
            SetAndValidateBaseProperties(dbModel, model);
            ImageHelper.Save(model, GetBaseUrl());
            dbModel.ImageUrl = model.ImageUrl;
            return dbModel;

        }
        public async Task<ItemModel> GetItem(int id)
        {
            return await (from i in _context.Items
                          join pi in _context.Items on i.ParentId equals pi.Id into tpi
                          from pi in tpi.DefaultIfEmpty()
                          join uom in _context.Uoms on i.DefaultUom equals uom.Id into tuom
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
        public async Task<List<ItemModel>> GetPeripheralItems(int? organizationId = null)
        {
            return await (from i in _context.Items
                          join pi in _context.Items on i.ParentId equals pi.Id into tpi
                          from pi in tpi.DefaultIfEmpty()
                          join uom in _context.Uoms on i.DefaultUom equals uom.Id into tuom
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
                              Type = (ItemTypeCatalog)(i.Type ?? 0),
                              Description = i.Description,
                              ImageUrl = i.ImageUrl,
                              IsPeripheral = i.IsPeripheral,
                              IsActive = i.IsActive,
                          }).ToListAsync();
        }
        public async Task<List<ItemModel>> GetRootItems()
        {
            return await (from i in _context.Items
                          join pi in _context.Items on i.ParentId equals pi.Id into tpi
                          from pi in tpi.DefaultIfEmpty()
                          join uom in _context.Uoms on i.DefaultUom equals uom.Id into tuom
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
        public async Task<IEnumerable<ItemModel>> GetAllItems(bool getHierarchicalData)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            var uoms = await _context.Uoms.Where(x => x.IsDeleted == false).ToListAsync();
            var result = await GetAllItems<ItemModel, ItemModel>(_context, true, getHierarchicalData);
            SetItemsUOM(uoms, result);
            return result;
        }
        private async Task<IEnumerable<T>> GetAllItems<T, M>(CharityContext _context, bool returnViewModel, bool getHierarchicalData)
            where T : class, IBase
            where M : class, ITree<M>
        {
            var itemsDBList = await _context.Items.FromSqlRaw(GetAllItemsTreeQuery()).ToListAsync();
            MapperConfiguration mapperConfig = GetItemMapperConfig();
            return GetAllNodes<T, Item, M>(mapperConfig, itemsDBList, returnViewModel, getHierarchicalData);

        }
        public async Task<IEnumerable<ItemModel>> GetSingleTreeItem(int id, bool getHierarchicalData)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            return await GetSingleItemTree<ItemModel, ItemModel>(_context, id, true, getHierarchicalData);
        }
        private async Task<IEnumerable<T>> GetSingleItemTree<T, M>(CharityContext _context, int id, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where M : class, ITree<M>
        {

            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            var ItemsDBList = await _context.Items.FromSqlRaw(GetItemTreeQuery(), new SqlParameter("@Id", id)).ToListAsync();
            MapperConfiguration mapperConfig = GetItemMapperConfig();
            var items = GetSingleNodeTree<T, Item, M>(id, mapperConfig, ItemsDBList, returnViewModel, getHierarchicalData);
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
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
               input => input.MapFrom(i => new UOMBriefModel { Id = i.DefaultUom ?? 0 }))
               .ForMember(dest => dest.Parent,
               input => input.MapFrom(i => new BaseBriefModel { Id = i.ParentId ?? 0 }))
               .ForMember(s => s.children, m => m.Ignore())
               );

        }
        private void SetItemsUOM(List<Uom> uoms, IEnumerable<ItemModel> items)
        {
            foreach (var item in items)
            {
                var uom = uoms.Where(x => x.Id == item.DefaultUOM.Id).FirstOrDefault();
                if (uom != null)
                {
                    item.DefaultUOM.Name = uom.Name;
                    item.DefaultUOM.NativeName = uom.NativeName;
                    item.DefaultUOM.NoOfBaseUnit = uom.NoOfBaseUnit;
                }
                if (item.children != null && item.children.Count > 0)
                {
                    SetItemsUOM(uoms, item.children);
                }
            }
        }
    }
}
