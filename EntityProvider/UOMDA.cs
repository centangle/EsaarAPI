using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using System;
using EntityProvider.Helpers;
using Models.BriefModel;
using AutoMapper;
using Models.Interfaces;
using Helpers;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateUOM(UOMModel model)
        {
            var dbModel = SetUOM(new Uom(), model);
            if (model.ParentId != 0)
                dbModel.ParentId = model.ParentId;
            if (model.RootId != 0)
                dbModel.RootId = model.RootId;
            _context.Uoms.Add(dbModel);
            await _context.SaveChangesAsync();
            model.Id = dbModel.Id;
            return model.Id;
        }
        public async Task<bool> UpdateUOM(UOMModel model)
        {
            Uom dbModel = await _context.Uoms.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (dbModel != null)
            {
                SetUOM(dbModel, model);
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
        public async Task<bool> CreateSingleUOMWithChildrens(UOMModel model)
        {
            var currentDbNodes = new List<Uom>();
            var allNodes = TreeHelper.TreeToList(new List<UOMModel> { model });
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Uoms, currentDbNodes, allNodes);
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
        public async Task<bool> UpdateSingleUOMWithChildren(UOMModel model)
        {

            var allNodes = TreeHelper.TreeToList(new List<UOMModel> { model });
            var currentRootNode = allNodes.Where(x => x.Node.ParentId == null || x.Node.ParentId == 0).Select(x => x.Node.Id).FirstOrDefault();
            var currentDbNodes = (await GetSingleUOMTree<Uom, Uom>(_context, currentRootNode, false, false)).ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Uoms, currentDbNodes, allNodes);
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
        public async Task<bool> CreateMultipleUOMsWithChildrens(List<UOMModel> uoms)
        {
            var currentDbNodes = new List<Uom>();
            var allNodes = TreeHelper.TreeToList(uoms);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Uoms, currentDbNodes, allNodes);
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
        public async Task<bool> UpdateMultipleUOMsWithChildrens(List<UOMModel> uoms)
        {
            var currentDbNodes = (await GetAllUOMs<Uom, Uom>(_context, false, false)).ToList();
            var allNodes = TreeHelper.TreeToList(uoms);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await ModifyTreeNodes(_context, _context.Uoms, currentDbNodes, allNodes);
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
        public async Task<bool> DeleteUOM(int Id)
        {
            Uom dbModel = await _context.Uoms.Where(x => x.Id == Id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (dbModel != null)
                if (dbModel != null)
                {
                    var root = (await GetSingleUOMTree<Uom, Uom>(_context, dbModel.Id, false)).First();
                    Dictionary<int, string> deletedUoms = new Dictionary<int, string>();
                    List<string> itemsUsingUoms = new List<string>();
                    DeleteTreeNode(root, deletedUoms);
                    foreach (var uom in deletedUoms)
                    {
                        List<string> usedItems = await _context.Items.Where(x => x.DefaultUom == uom.Key && x.IsDeleted == false).Select(x => x.Name).ToListAsync();
                        if (usedItems.Count > 0)
                        {
                            itemsUsingUoms.Add($"{uom.Value} is used in {string.Join(", ", usedItems)}");
                        }
                    }
                    if (itemsUsingUoms.Count > 0)
                    {
                        throw new KnownException($"Can not delete this uom because {string.Join(", and ", itemsUsingUoms)}.");
                    }
                    else
                    {
                        return await _context.SaveChangesAsync() > 0;
                    }
                }
            return false;
        }
        public async Task<UOMModel> GetUOM(int id)
        {
            PaginatedResultModel<UOMModel> searchResult = new PaginatedResultModel<UOMModel>();
            UOMModel uom = await (from u in _context.Uoms
                                  where u.Id == id
                                   && (u.ParentId == null)
                                   && u.IsDeleted == false
                                  select new UOMModel
                                  {
                                      Id = u.Id,
                                      Name = u.Name,
                                      NativeName = u.NativeName,
                                      Abbreviation = u.Abbreviation,
                                      NoOfBaseUnit = u.NoOfBaseUnit,
                                      Description = u.Description,
                                      ParentId = null,
                                  }).FirstOrDefaultAsync();
            if (uom != null)
            {
                uom.children = await (from u in _context.Uoms
                                      where u.ParentId == uom.Id
                                      && u.IsDeleted == false
                                      select new UOMModel
                                      {
                                          Id = u.Id,
                                          Name = u.Name,
                                          NativeName = u.NativeName,
                                          Abbreviation = u.Abbreviation,
                                          NoOfBaseUnit = u.NoOfBaseUnit,
                                          Description = u.Description,
                                          ParentId = u.ParentId,
                                          Parent = new BaseBriefModel()
                                          {
                                              Id = u.ParentId ?? 0,
                                          },
                                          Root = new BaseBriefModel()
                                          {
                                              Id = u.RootId ?? 0,
                                          },
                                      }).ToListAsync();
            }
            return uom;
        }
        public async Task<PaginatedResultModel<UOMModel>> GetUOM(UOMSearchModel filters)
        {
            PaginatedResultModel<UOMModel> searchResult = new PaginatedResultModel<UOMModel>();
            IQueryable<UOMModel> UOMsQueryable = (from u in _context.Uoms
                                                  where (string.IsNullOrEmpty(filters.Name) || u.Name.ToLower().Contains(filters.Name.ToLower()))
                                                   && (u.ParentId == null)
                                                   && u.IsDeleted == false
                                                  select new UOMModel
                                                  {
                                                      Id = u.Id,
                                                      Name = u.Name,
                                                      NativeName = u.NativeName,
                                                      Abbreviation = u.Abbreviation,
                                                      NoOfBaseUnit = u.NoOfBaseUnit,
                                                      Description = u.Description,
                                                      ParentId = null,
                                                  }).AsQueryable();
            List<UOMModel> UOMList = await UOMsQueryable.OrderBy(x => x.Name).Skip((filters.CurrentPage - 1) * filters.RecordsPerPage).Take(filters.RecordsPerPage).ToListAsync();
            foreach (var uom in UOMList)
            {
                uom.children = await (from u in _context.Uoms
                                      where u.ParentId == uom.Id
                                      && u.IsDeleted == false
                                      select new UOMModel
                                      {
                                          Id = u.Id,
                                          Name = u.Name,
                                          NativeName = u.NativeName,
                                          Abbreviation = u.Abbreviation,
                                          NoOfBaseUnit = u.NoOfBaseUnit,
                                          Description = u.Description,
                                          ParentId = u.ParentId,
                                          Parent = new BaseBriefModel()
                                          {
                                              Id = u.ParentId ?? 0,
                                          },
                                          Root = new BaseBriefModel()
                                          {
                                              Id = u.RootId ?? 0,
                                          },
                                      }).ToListAsync();
            }
            searchResult.Items = UOMList ?? new List<UOMModel>();
            if (filters.CalculateTotal)
            {
                searchResult.TotalCount = UOMsQueryable == null ? 0 : UOMsQueryable.Count();
            }
            return searchResult;
        }
        public async Task<PaginatedResultModel<UOMModel>> GetUOMForDD(UOMSearchModel filters)
        {
            PaginatedResultModel<UOMModel> searchResult = new PaginatedResultModel<UOMModel>();
            List<UOMModel> UOMList = new List<UOMModel>();
            ////======================Get Only Parent UOMS====================
            //UOMList = await (from u in _context.Uoms
            //                 where
            //                 (string.IsNullOrEmpty(filters.Name) || u.Name.Contains(filters.Name) || u.Abbreviation.Contains(filters.Name))
            //                 && ((filters.ParentId == null || u.Id == filters.ParentId) && u.ParentId == null)
            //                 && u.IsDeleted == false
            //                 select new UOMModel
            //                 {
            //                     Id = u.Id,
            //                     Name = u.Name,
            //                     NativeName = u.NativeName,
            //                     NoOfBaseUnit = u.NoOfBaseUnit,
            //                     Description = u.Description,
            //                 }).ToListAsync();
            ////======================Get Only Child UOMS====================

            UOMList.AddRange(await (from u in _context.Uoms
                                    where
                                    (string.IsNullOrEmpty(filters.Name) || u.Name.Contains(filters.Name) || u.Abbreviation.Contains(filters.Name))
                                    && ((filters.ParentId == null || u.ParentId == filters.ParentId) && u.ParentId != null)
                                    && u.IsDeleted == false
                                    && u.IsPeripheral == true
                                    select new UOMModel
                                    {
                                        Id = u.Id,
                                        Name = u.Name,
                                        NativeName = u.NativeName,
                                        ParentId = u.ParentId,
                                        Parent = new BaseBriefModel()
                                        {
                                            Id = u.ParentId ?? 0,
                                        },
                                        Root = new BaseBriefModel()
                                        {
                                            Id = u.RootId ?? 0,
                                        },
                                        NoOfBaseUnit = u.NoOfBaseUnit,
                                        Description = u.Description,
                                    }).ToListAsync());
            searchResult.Items = UOMList ?? new List<UOMModel>();
            if (filters.CalculateTotal)
            {
                searchResult.TotalCount = UOMList.Count();
            }
            return searchResult;
        }
        private Uom SetUOM(Uom dbModel, UOMModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Abbreviation = model.Abbreviation;
            dbModel.NoOfBaseUnit = model.NoOfBaseUnit;
            dbModel.Type = (int)model.Type;
            //Set Root
            if (model.Root == null || model.Root.Id == 0)
                dbModel.RootId = null;
            else
                dbModel.RootId = model.Root.Id;
            dbModel.Description = model.Description;
            dbModel.IsPeripheral = model.IsPeripheral;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        private List<UOMBriefModel> GetUOMTreeFlatList(UOMModel uom)
        {
            List<UOMBriefModel> uomtreeFlatList = new List<UOMBriefModel>();
            UOMBriefModel uomBM = new UOMBriefModel
            {
                Id = uom.Id,
                Name = uom.Name,
                NativeName = uom.NativeName,
                NoOfBaseUnit = uom.NoOfBaseUnit,
            };
            uomtreeFlatList.Add(uomBM);
            uomtreeFlatList.AddRange((from u in uom.children
                                      select new UOMBriefModel
                                      {
                                          Id = u.Id,
                                          Name = u.Name,
                                          NativeName = u.NativeName,
                                          NoOfBaseUnit = u.NoOfBaseUnit,
                                      }).ToList());
            return uomtreeFlatList;
        }
        public async Task<List<UOMModel>> GetPeripheralUOMs()
        {
            return await (from uom in _context.Uoms
                          join puom in _context.Uoms on uom.ParentId equals puom.Id into tpuom
                          from puom in tpuom.DefaultIfEmpty()
                          where uom.IsPeripheral == true
                          && uom.IsDeleted == false
                          select new UOMModel
                          {
                              Id = uom.Id,
                              Parent = new BaseBriefModel()
                              {
                                  Id = puom == null ? 0 : puom.Id,
                                  Name = puom == null ? "" : puom.Name,
                                  NativeName = puom == null ? "" : puom.NativeName
                              },
                              Name = uom.Name,
                              NativeName = uom.NativeName,
                              NoOfBaseUnit = uom.NoOfBaseUnit,
                              Description = uom.Description,
                              IsPeripheral = uom.IsPeripheral,
                              IsActive = uom.IsActive,
                          }).ToListAsync();
        }
        public async Task<List<UOMModel>> GetRootUOMs()
        {
            return await (from uom in _context.Uoms
                          join puom in _context.Uoms on uom.ParentId equals puom.Id into tpuom
                          from puom in tpuom.DefaultIfEmpty()
                          where uom.ParentId == null
                          && uom.IsDeleted == false
                          select new UOMModel
                          {
                              Id = uom.Id,
                              Parent = new BaseBriefModel()
                              {
                                  Id = puom == null ? 0 : puom.Id,
                                  Name = puom == null ? "" : puom.Name,
                                  NativeName = puom == null ? "" : puom.NativeName
                              },
                              Name = uom.Name,
                              NativeName = uom.NativeName,
                              NoOfBaseUnit = uom.NoOfBaseUnit,
                              IsPeripheral = uom.IsPeripheral,
                              IsActive = uom.IsActive,
                              Description = uom.Description,
                          }).ToListAsync();
        }
        public async Task<IEnumerable<UOMModel>> GetAllUOMs(bool getHierarchicalData)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            return await GetAllUOMs<UOMModel, UOMModel>(_context, true, getHierarchicalData);
        }
        private async Task<IEnumerable<T>> GetAllUOMs<T, M>(CharityContext _context, bool returnViewModel, bool getHierarchicalData)
            where T : class, IBase
            where M : class, ITree<M>
        {
            var uOMsDBList = await _context.Uoms.FromSqlRaw(GetAllUOMsTreeQuery()).ToListAsync();
            MapperConfiguration mapperConfig = GetUOMMapperConfig();
            return GetAllNodes<T, Uom, M>(mapperConfig, uOMsDBList, returnViewModel, getHierarchicalData);

        }
        public async Task<IEnumerable<UOMModel>> GetSingleTreeUOM(int id, bool getHierarchicalData)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            return await GetSingleUOMTree<UOMModel, UOMModel>(_context, id, true, getHierarchicalData);
        }
        private async Task<IEnumerable<T>> GetSingleUOMTree<T, M>(CharityContext _context, int id, bool returnViewModel = true, bool getHierarchicalData = true)
            where T : class, IBase
            where M : class, ITree<M>
        {

            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            var UOMsDBList = await _context.Uoms.FromSqlRaw(GetUOMTreeQuery(), new SqlParameter("@Id", id)).ToListAsync();
            MapperConfiguration mapperConfig = GetUOMMapperConfig();
            var uOMs = GetSingleNodeTree<T, Uom, M>(id, mapperConfig, UOMsDBList, returnViewModel, getHierarchicalData);
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
            return uOMs;
        }
        private string GetAllUOMsTreeQuery()
        {
            return @"
                        WITH cte As
                        (
                            SELECT ParentUOM.*
                            FROM Uom ParentUOM
                            WHERE ParentId is null
                            and IsDeleted=0
                            UNION All
                                SELECT ChildUOM.*
                                FROM Uom ChildUOM
                                JOIN cte
                                On cte.id = ChildUOM.ParentId
                                where ChildUOM.IsDeleted=0
                        )
                        SELECT*
                        FROM cte";
        }
        private string GetUOMTreeQuery()
        {
            return @"
                        WITH cte As
                        (
                            SELECT ParentUOM.*
                            FROM Uom ParentUOM
                            WHERE Id = @Id
                            and IsDeleted=0
                            UNION All
                                SELECT ChildUOM.*
                                FROM Uom ChildUOM
                                JOIN cte
                                On cte.id = ChildUOM.ParentId
                                where ChildUOM.IsDeleted=0
                        )
                        SELECT*
                        FROM cte";
        }
        private MapperConfiguration GetUOMMapperConfig()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<Uom, UOMModel>()
               .ForMember(dest => dest.Parent,
               input => input.MapFrom(i => new BaseBriefModel { Id = i.ParentId ?? 0 }))
               .ForMember(s => s.children, m => m.Ignore())
               );

        }
    }
}
