using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<int> AddPackage(PackageModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                if (await IsPackgeValidated(context, model))
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            model.IsPeripheral = true;
                            var dbModel = SetItem(new Item(), model);
                            context.Items.Add(dbModel);
                            bool result = await context.SaveChangesAsync() > 0;
                            if (result)
                            {
                                model.Id = dbModel.Id;
                                AddPackageItems(context, model.children, model.Id);
                                await context.SaveChangesAsync();
                            }
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
            return 0;
        }
        public async Task<bool> UpdatePackage(PackageModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                if (await IsPackgeValidated(context, model))
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            Item dbModel = await context.Items.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                            if (dbModel != null)
                            {
                                model.IsPeripheral = true;
                                SetItem(dbModel, model);
                                await ModifyPackageItems(context, model);
                                bool result = await context.SaveChangesAsync() > 0 ;
                                transaction.Commit();
                                return result;
                            }
                            return false;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }

                }
            }
            return false;
        }
        public async Task<bool> DeletePackage(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Item dbModel = await context.Items.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        public async Task<PackageModel> GetPackage(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var package = await (from p in context.Items
                                     join o in context.Organizations on p.OrganizationId equals o.Id
                                     join uom in context.UOMs on p.DefaultUOM equals uom.Id
                                     where p.Id == id
                                     && p.IsDeleted == false
                                     select new PackageModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         NativeName = p.NativeName,
                                         Organization = new BaseBriefModel()
                                         {
                                             Id = o.Id,
                                             Name = o.Name,
                                             NativeName = o.NativeName,
                                         },
                                         DefaultUOM = new UOMBriefModel()
                                         {
                                             Id = uom.Id,
                                             Name = uom.Name,
                                             NativeName = uom.NativeName,
                                             NoOfBaseUnit = uom.NoOfBaseUnit,
                                         },
                                         Type = (Catalogs.ItemTypeCatalog)p.Type,
                                         Description = p.Description,
                                         ImageUrl = p.ImageUrl,
                                         IsPeripheral = p.IsPeripheral,
                                         IsActive = p.IsActive,
                                     }).FirstOrDefaultAsync();
                if (package != null)
                {
                    package.children = await (from pi in context.PackageItems
                                              join i in context.Items on pi.ItemId equals i.Id
                                              join uom in context.UOMs on pi.ItemUOM equals uom.Id
                                              where pi.PackageId == package.Id
                                              && pi.IsDeleted == false
                                              select new PackageItemModel
                                              {
                                                  Id = pi.Id,
                                                  Item = new BaseBriefModel
                                                  {
                                                      Id = i.Id,
                                                      Name = i.Name,
                                                      NativeName = i.NativeName,
                                                  },
                                                  ItemUOM = new UOMBriefModel()
                                                  {
                                                      Id = uom.Id,
                                                      Name = uom.Name,
                                                      NativeName = uom.NativeName,
                                                      NoOfBaseUnit = uom.NoOfBaseUnit,
                                                  },
                                                  ItemQuantity = pi.ItemQuantity,
                                                  IsActive = pi.IsActive,
                                              }).ToListAsync();
                }
                return package;
            }
        }
        private void AddPackageItems(CharityEntities context, ICollection<PackageItemModel> packageItems, int packageId)
        {
            foreach (var item in packageItems)
            {
                var dbModel = SetPackageItem(new PackageItem(), item, packageId);
                context.PackageItems.Add(dbModel);
            }
        }
        private void UpdatePackageItems(IEnumerable<PackageItem> modfiedItems, IEnumerable<PackageItemModel> packageItems,int packageId)
        {
            foreach (var dbModel in modfiedItems)
            {
                PackageItemModel model = packageItems.Where(x => x.Id == dbModel.Id).FirstOrDefault();
                SetPackageItem(dbModel, model, packageId);
            }
        }
        private void DeletePackageItems(IEnumerable<PackageItem> deletedItems)
        {
            foreach (var item in deletedItems)
            {
                item.IsDeleted = true;
            }
        }
        private async Task ModifyPackageItems(CharityEntities context, PackageModel model)
        {
            var masterList = await context.PackageItems.Where(x => x.PackageId == model.Id).ToListAsync();
            var newItems = model.children.Where(x => x.Id == 0).ToList();
            var updatedItems = masterList.Where(m => model.children.Any(s => m.Id == s.Id));
            var deletedItems = masterList.Where(m => !model.children.Any(s => m.Id == s.Id));
            AddPackageItems(context, newItems, model.Id);
            UpdatePackageItems(updatedItems, model.children, model.Id);
            DeletePackageItems(deletedItems);
        }
        private PackageItem SetPackageItem(PackageItem dbModel, PackageItemModel model, int packageId)
        {
            if (model.ItemUOM == null || model.ItemUOM.Id == 0)
            {
                throw new KnownException("Item UOM is required");
            }
            dbModel.PackageId = packageId;
            dbModel.ItemId = model.Item.Id;
            dbModel.ItemQuantity = model.ItemQuantity;
            dbModel.ItemUOM = model.ItemUOM.Id;
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
        private async Task<bool> IsPackgeValidated(CharityEntities context, PackageModel model)
        {
            if (model.Organization == null || model.Organization.Id == 0)
            {
                throw new KnownException("Package Organization is required");
            }
            var organizationMember = (await GetMemberRoleForOrganization(context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(organizationMember) == false)
            {
                throw new KnownException("You are not authorized to add package to this organization");
            }
            if (model.DefaultUOM == null || model.DefaultUOM.Id == 0)
            {
                throw new KnownException("Package UOM is required");
            }
            return true;
        }

    }
}
