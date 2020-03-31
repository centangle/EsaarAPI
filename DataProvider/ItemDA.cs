using AutoMapper;
using Catalogs;
using Dapper;
using DataProvider.Helpers;
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
        public async Task<int> CreateItem(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetItem(new Item(), model, true);
                if (model.ParentId != 0)
                    dbModel.ParentId = model.ParentId;
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
                Item dbModel = await context.Items.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetItem(dbModel, model, false);
                    if (model.ParentId != 0)
                        dbModel.ParentId = model.ParentId;
                    else
                        dbModel.ParentId = null;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<int> CreateSingleItemWithChildrens(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var dbModel = AddTreeItem<Item, ItemModel>(model, true);
                        context.Items.Add(dbModel);
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
        public async Task<bool> UpdateSingleItemWithChildren(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var root = (await GetSingleItemTree<Item, Item>(context, model.Id, false)).First();
                var itemToUpdate = context.Items.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefault();
                if (itemToUpdate != null)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            UpdateTreeItem(context, root, model);
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
        public async Task<bool> CreateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            var dbModel = AddTreeItem<Item, ItemModel>(item, true);
                            context.Items.Add(dbModel);
                            await context.SaveChangesAsync();
                            item.Id = dbModel.Id;
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
        public async Task<bool> UpdateMultipleItemsWithChildrens(List<ItemModel> items)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            var root = (await GetSingleItemTree<Item, Item>(context, item.Id, false)).First();
                            var itemToUpdate = context.Items.Where(x => x.Id == item.Id && x.IsDeleted == false).FirstOrDefault();
                            if (itemToUpdate != null)
                            {
                                UpdateTreeItem(context, root, item);
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
        public async Task<bool> DeleteItem(int Id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Item dbModel = await context.Items.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    var root = (await GetSingleItemTree<Item, Item>(context, dbModel.Id, false)).First();
                    DeleteItemTree(root);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        private Item SetItem(Item dbModel, ItemModel model, bool isNew)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            if (model.DefaultUOM == null)
                dbModel.DefaultUOM = null;
            else
                dbModel.DefaultUOM = model.DefaultUOM.Id;
            dbModel.Description = model.Description;
            dbModel.IsCartItem = model.IsCartItem;
            dbModel.IsActive = model.IsActive;
            if (isNew)
                dbModel.IsDeleted = false;
            else
                dbModel.IsDeleted = model.IsDeleted;
            ImageHelper.Save(model);
            dbModel.ImageUrl = model.ImageUrl;
            return dbModel;

        }
        public async Task<ItemModel> GetItem(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from i in context.Items
                              join pi in context.UOMs on i.ParentId equals pi.Id into tpi
                              from pi in tpi.DefaultIfEmpty()
                              join uom in context.UOMs on i.DefaultUOM equals uom.Id into tuom
                              from uom in tuom.DefaultIfEmpty()
                              where i.Id == id
                              && i.IsDeleted == false
                              select new ItemModel
                              {
                                  Id = i.Id,
                                  Parent = new BriefModel()
                                  {
                                      Id = pi == null ? 0 : pi.Id,
                                      Name = pi == null ? "" : pi.Name,
                                      NativeName = pi == null ? "" : pi.NativeName
                                  },
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  DefaultUOM = new BriefModel()
                                  {
                                      Id = uom == null ? 0 : uom.Id,
                                      Name = uom == null ? "" : uom.Name,
                                      NativeName = uom == null ? "" : uom.NativeName
                                  },

                                  Description = i.Description,
                                  Type = (ItemTypeCatalog)(i.Type ?? 0),
                                  ImageUrl = i.ImageUrl,
                                  IsCartItem = i.IsCartItem,
                                  IsActive = i.IsActive,
                              }).FirstOrDefaultAsync();
            }
        }

        public async Task<List<ItemModel>> GetPeripheralItems()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from i in context.Items
                              join pi in context.UOMs on i.ParentId equals pi.Id into tpi
                              from pi in tpi.DefaultIfEmpty()
                              join uom in context.UOMs on i.DefaultUOM equals uom.Id into tuom
                              from uom in tuom.DefaultIfEmpty()
                              where i.IsCartItem == true
                              && i.IsDeleted == false
                              select new ItemModel
                              {
                                  Id = i.Id,
                                  Parent = new BriefModel()
                                  {
                                      Id = pi == null ? 0 : pi.Id,
                                      Name = pi == null ? "" : pi.Name,
                                      NativeName = pi == null ? "" : pi.NativeName
                                  },
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  DefaultUOM = new BriefModel()
                                  {
                                      Id = uom == null ? 0 : uom.Id,
                                      Name = uom == null ? "" : uom.Name,
                                      NativeName = uom == null ? "" : uom.NativeName
                                  },

                                  Description = i.Description,
                                  Type = (ItemTypeCatalog)(i.Type ?? 0),
                                  ImageUrl = i.ImageUrl,
                                  IsCartItem = i.IsCartItem,
                                  IsActive = i.IsActive,
                              }).ToListAsync();
            }
        }

        public async Task<List<ItemModel>> GetRootItems()
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from i in context.Items
                              join pi in context.UOMs on i.ParentId equals pi.Id into tpi
                              from pi in tpi.DefaultIfEmpty()
                              join uom in context.UOMs on i.DefaultUOM equals uom.Id into tuom
                              from uom in tuom.DefaultIfEmpty()
                              where i.ParentId == null
                              && i.IsDeleted == false
                              select new ItemModel
                              {
                                  Id = i.Id,
                                  Parent = new BriefModel()
                                  {
                                      Id = pi == null ? 0 : pi.Id,
                                      Name = pi == null ? "" : pi.Name,
                                      NativeName = pi == null ? "" : pi.NativeName
                                  },
                                  Name = i.Name,
                                  NativeName = i.NativeName,
                                  DefaultUOM = new BriefModel()
                                  {
                                      Id = uom == null ? 0 : uom.Id,
                                      Name = uom == null ? "" : uom.Name,
                                      NativeName = uom == null ? "" : uom.NativeName
                                  },

                                  Description = i.Description,
                                  Type = (ItemTypeCatalog)(i.Type ?? 0),
                                  ImageUrl = i.ImageUrl,
                                  IsCartItem = i.IsCartItem,
                                  IsActive = i.IsActive,
                              }).ToListAsync();
            }
        }

        public async Task<IEnumerable<ItemModel>> GetAllItems(bool getHierarchicalData)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var itemsDBList = await context.Items.SqlQuery(GetAllItemsTreeQuery()).AsNoTracking().ToListAsync();
                MapperConfiguration mapperConfig = GetItemMapperConfig();
                return GetAllNodes<ItemModel, Item, ItemModel>(mapperConfig, itemsDBList, true, getHierarchicalData);
            }

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
               input => input.MapFrom(i => new BriefModel { Id = i.DefaultUOM ?? 0 }))
               .ForMember(dest => dest.Parent,
               input => input.MapFrom(i => new BriefModel { Id = i.ParentId ?? 0 }))
               .ForMember(s => s.Childrens, m => m.Ignore())
               );

        }
    }
}
