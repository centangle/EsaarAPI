using AutoMapper;
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
        public async Task<int> AddSingleItem(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = AddTreeItem<Item, ItemModel>(model, true);
                context.Items.Add(dbModel);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        public async Task<bool> UpdateSingleItem(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Item dbModel = await context.Items.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetItem(dbModel, model, false);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<int> AddMultipleChildItem(ItemModel model)
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
        public async Task<bool> UpdateMultipleChildItem(ItemModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var root = (await GetSingleItemTreeEF<Item, Item>(context, model.Id, false)).First();
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
        public async Task<bool> DeleteItem(int Id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Item dbModel = await context.Items.Where(x => x.Id == Id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        private Item SetItem(Item dbModel, ItemModel model, bool isNew)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
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

        public async Task<PaginatedResultModel<UOMModel>> GetItem(UOMSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                PaginatedResultModel<UOMModel> searchResult = new PaginatedResultModel<UOMModel>();
                IQueryable<UOMModel> UOMsQueryable = (from u in context.UOMs
                                                      where (string.IsNullOrEmpty(filters.Name) || u.Name.ToLower().Contains(filters.Name.ToLower()))
                                                       && (u.ParentId == null)
                                                       && u.IsDeleted == false
                                                      select new UOMModel
                                                      {
                                                          Id = u.Id,
                                                          Name = u.Name,
                                                          Abbreviation = u.Abbreviation,
                                                          NoOfBaseUnit = u.NoOfBaseUnit,
                                                          ParentId = null,
                                                      }).AsQueryable();
                List<UOMModel> UOMList = await UOMsQueryable.OrderBy(x => x.Name).Skip((filters.CurrentPage - 1) * filters.RecordsPerPage).Take(filters.RecordsPerPage).ToListAsync();
                foreach (var uom in UOMList)
                {
                    uom.Childrens = await (from u in context.UOMs
                                           where u.ParentId == uom.Id
                                           select new UOMModel
                                           {
                                               Id = u.Id,
                                               Name = u.Name,
                                               Abbreviation = u.Abbreviation,
                                               NoOfBaseUnit = u.NoOfBaseUnit,
                                               ParentId = u.ParentId,
                                           }).ToListAsync();
                }
                searchResult.Items = UOMList ?? new List<UOMModel>();
                if (filters.CalculateTotal)
                {
                    searchResult.TotalCount = UOMsQueryable == null ? 0 : UOMsQueryable.Count();
                }
                return searchResult;
            }
        }

        public async Task<IEnumerable<T>> GetSingleItemTreeEF<T, M>(CharityEntities context, int id, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where M : class, ITree<M>
        {
            var ItemsDBList = await context.Items.SqlQuery(GetItemTreeQuery(), new SqlParameter("@Id", id)).ToListAsync();
            MapperConfiguration mapperConfig = GetItemMapperConfig();
            return TreeHelper.GetTreeData<T, Item, M>(ItemsDBList, returnViewModel, getHierarchicalData, mapperConfig);
        }
        public async Task<IEnumerable<T>> GetSingleItemTree<T, M>(int id, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where M : class, ITree<M>
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnectionStringValue()))
            {
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", id);
                var ItemsDBList = await connection.QueryAsync<Item>(GetItemTreeQuery(), queryParameters);
                MapperConfiguration mapperConfig = GetItemMapperConfig();
                return TreeHelper.GetTreeData<T, Item, M>(ItemsDBList, returnViewModel, getHierarchicalData, mapperConfig);
            }
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
               input => input.MapFrom(i => new BriefModel { Id = i.DefaultUOM })));
        }
    }
}
