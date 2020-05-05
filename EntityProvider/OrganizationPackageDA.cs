using Helpers;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> AddPackage(PackageModel model)
        {
            if (await IsPackgeValidated(_context, model))
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        model.IsPeripheral = true;
                        var dbModel = SetItem(new Item(), model);
                        _context.Items.Add(dbModel);
                        bool result = await _context.SaveChangesAsync() > 0;
                        if (result)
                        {
                            model.Id = dbModel.Id;
                            AddPackageItems(_context, model.Items, model.Id);
                            var organizationItemModel = GetOrganizationItemFromPackage(model);
                            await CreateOrganizationItem(_context, organizationItemModel);
                            await _context.SaveChangesAsync();
                        }
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
            return 0;
        }
        public async Task<bool> UpdatePackage(PackageModel model)
        {
            if (await IsPackgeValidated(_context, model))
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Item dbModel = await _context.Items.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (dbModel != null)
                        {
                            model.IsPeripheral = true;
                            SetItem(dbModel, model);
                            await ModifyPackageItems(_context, model);
                            bool result = await _context.SaveChangesAsync() > 0;
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
            return false;
        }
        public async Task<bool> DeletePackage(int id)
        {
            Item dbModel = await _context.Items.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (dbModel != null)
            {
                var memberOrgRoles = (await GetMemberRoleForOrganization(_context, dbModel.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(memberOrgRoles))
                {
                    dbModel.IsDeleted = true;
                    return await _context.SaveChangesAsync() > 0;
                }
            }
            else
                throw new KnownException("You are not authorized to perform this action");
            return false;
        }
        public async Task<PackageModel> GetPackage(int id)
        {
            var package = await (from p in _context.Items
                                 join o in _context.Organizations on p.OrganizationId equals o.Id
                                 join uom in _context.Uoms on p.DefaultUom equals uom.Id
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
                package.Items = await (from pi in _context.PackageItems
                                       join i in _context.Items on pi.ItemId equals i.Id
                                       join uom in _context.Uoms on pi.ItemUom equals uom.Id
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

        private void AddPackageItems(CharityContext _context, ICollection<PackageItemModel> packageItems, int packageId)
        {
            foreach (var item in packageItems)
            {
                var dbModel = SetPackageItem(new PackageItem(), item, packageId);
                _context.PackageItems.Add(dbModel);
            }
        }
        private void UpdatePackageItems(IEnumerable<PackageItem> modfiedItems, IEnumerable<PackageItemModel> packageItems, int packageId)
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
        private async Task ModifyPackageItems(CharityContext _context, PackageModel model)
        {
            var masterList = await _context.PackageItems.Where(x => x.PackageId == model.Id).ToListAsync();
            var newItems = model.Items.Where(x => x.Id == 0).ToList();
            var updatedItems = masterList.Where(m => model.Items.Any(s => m.Id == s.Id));
            var deletedItems = masterList.Where(m => !model.Items.Any(s => m.Id == s.Id));
            AddPackageItems(_context, newItems, model.Id);
            UpdatePackageItems(updatedItems, model.Items, model.Id);
            DeletePackageItems(deletedItems);
        }
        private PackageItem SetPackageItem(PackageItem dbModel, PackageItemModel model, int packageId)
        {
            if (model.ItemUOM == null || model.ItemUOM.Id == 0)
            {
                throw new KnownException("Item Uom is required");
            }
            dbModel.PackageId = packageId;
            dbModel.ItemId = model.Item.Id;
            dbModel.ItemQuantity = model.ItemQuantity;
            dbModel.ItemUom = model.ItemUOM.Id;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
        private async Task<bool> IsPackgeValidated(CharityContext _context, PackageModel model)
        {
            if (model.Organization == null || model.Organization.Id == 0)
            {
                throw new KnownException("Package Organization is required");
            }
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles) == false)
            {
                throw new KnownException("You are not authorized to add package to this organization");
            }
            if (model.DefaultUOM == null || model.DefaultUOM.Id == 0)
            {
                throw new KnownException("Package Uom is required");
            }
            return true;
        }
        private OrganizationItemModel GetOrganizationItemFromPackage(PackageModel model)
        {
            OrganizationItemModel itemModel = new OrganizationItemModel();
            itemModel.Item.Id = model.Id;
            itemModel.Organization.Id = model.Organization.Id;
            return itemModel;
        }

    }
}
