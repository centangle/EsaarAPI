using Catalogs;
using Helpers;
using Models;
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
        private async Task<bool> AddDonationRequestOrganizationItems(CharityContext _context, List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            foreach (var item in items)
            {
                var dbModel = await _context.DonationRequestOrganizationItems.Where(x => x.RequestOrganizationId == donationRequestOrganizationId && x.RequestItemId == item.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel == null)
                {
                    dbModel = SetDonationRequestOrganizationItem(new DonationRequestOrganizationItem(), item, donationRequestOrganizationId, StatusCatalog.Approved);
                    _context.DonationRequestOrganizationItems.Add(dbModel);
                    item.Id = dbModel.Id;
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        private async Task<bool> UpdateDonationRequestOrganizationItems(CharityContext _context, List<DonationRequestOrganizationItemModel> requestItems, int donationRequestOrganizationId, StatusCatalog status)
        {
            foreach (var requestItem in requestItems)
            {
                var dbModel = await _context.DonationRequestOrganizationItems.Where(x => x.RequestOrganizationId == donationRequestOrganizationId && x.RequestItemId == requestItem.Item.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetDonationRequestOrganizationItem(dbModel, requestItem, donationRequestOrganizationId, status);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        private DonationRequestOrganizationItem SetDonationRequestOrganizationItem(DonationRequestOrganizationItem dbModel, DonationRequestOrganizationItemModel model, int donationRequestOrganizationId, StatusCatalog status)
        {
            dbModel.RequestOrganizationId = donationRequestOrganizationId;
            if (model.Item == null || model.Item.Id < 1)
            {
                throw new KnownException("Item is required");
            }
            dbModel.RequestItemId = model.Item.Id;
            if (status == StatusCatalog.Approved && (model.ApprovedQuantity == null || model.ApprovedQuantity < 0))
            {
                throw new KnownException("Approved Quantity is required");
            }
            if (status == StatusCatalog.Collected && (model.CollectedQuantity == null || model.CollectedQuantity < 0))
            {
                throw new KnownException("Collected Quantity is required");
            }
            if (status == StatusCatalog.Delivered && (model.DeliveredQuantity == null || model.DeliveredQuantity < 0))
            {
                throw new KnownException("Delivered Quantity is required");
            }
            if (model.ApprovedQuantity != null && model.ApprovedQuantity > 0)
            {
                dbModel.Quantity = model.ApprovedQuantity ?? 0;
                if (model.ApprovedQuantityUOM == null || model.ApprovedQuantityUOM.Id < 1)
                {
                    throw new KnownException("Approved Quantity Uom is required");
                }
                else
                {
                    dbModel.QuantityUom = model.ApprovedQuantityUOM.Id;
                }
            }
            if (model.CollectedQuantity != null)
            {
                dbModel.CollectedQuantity = model.CollectedQuantity;
                dbModel.CollectionVolunteerId = _loggedInMemberId;
                dbModel.CollectionDate = DateTime.UtcNow;
                dbModel.CollectionLatLong = "";
                if (model.CollectedQuantity > 0 && (model.CollectedQuantityUOM == null || model.CollectedQuantityUOM.Id < 1))
                {
                    throw new KnownException("Collected Quantity Uom is required");
                }
                else
                {
                    dbModel.CollectedQuantityUom = model.CollectedQuantityUOM.Id;
                }
            }

            if (model.DeliveredQuantity != null)
            {
                dbModel.DeliveredQuantity = model.DeliveredQuantity;
                dbModel.DeliveryVolunteerId = _loggedInMemberId;
                dbModel.DeliveryDate = DateTime.UtcNow;
                dbModel.DeliveryLatLong = "";
                if (model.DeliveredQuantity > 0 && (model.DeliveredQuantityUOM == null || model.DeliveredQuantityUOM.Id < 1))
                {
                    throw new KnownException("Delivered Quantity UOM is required");
                }
                else
                {
                    dbModel.DeliveredQuantityUom = model.DeliveredQuantityUOM.Id;
                }
            }
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}
