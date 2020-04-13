﻿using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<bool> AddDonationRequestOrganizationItems(List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                foreach (var item in items)
                {
                    var dbModel = await context.DonationRequestOrganizationItems.Where(x => x.RequestOrganizationId == donationRequestOrganizationId && x.RequestItemId == item.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (dbModel == null)
                    {
                        dbModel = SetDonationRequestOrganizationItem(new DonationRequestOrganizationItem(), item, donationRequestOrganizationId);
                        context.DonationRequestOrganizationItems.Add(dbModel);
                        item.Id = dbModel.Id;
                    }
                }
                await context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> UpdateDonationRequestOrganizationItems(List<DonationRequestOrganizationItemModel> requestItems, int donationRequestOrganizationId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                foreach (var requestItem in requestItems)
                {
                    var dbModel = await context.DonationRequestOrganizationItems.Where(x => x.RequestOrganizationId == donationRequestOrganizationId && x.RequestItemId == requestItem.Item.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        SetDonationRequestOrganizationItem(dbModel, requestItem, donationRequestOrganizationId);
                    }
                }
                await context.SaveChangesAsync();
                return true;
            }
        }
        private DonationRequestOrganizationItem SetDonationRequestOrganizationItem(DonationRequestOrganizationItem dbModel, DonationRequestOrganizationItemModel model, int donationRequestOrganizationId)
        {
            dbModel.RequestOrganizationId = donationRequestOrganizationId;
            if (model.Item == null || model.Item.Id < 1)
            {
                throw new KnownException("Item is required");
            }
            dbModel.RequestItemId = model.Item.Id;
            if (dbModel.Id == 0 && (model.Quantity == 0 || model.Quantity < 0))
            {
                throw new KnownException("Quantity is required");
            }
            if (model.Quantity != null && model.Quantity > 0)
            {
                dbModel.Quantity = model.Quantity ?? 0;
                if (model.QuantityUOM == null || model.QuantityUOM.Id < 1)
                {
                    throw new KnownException("Quantity UOM is required");
                }
                else
                {
                    dbModel.QuantityUOM = model.QuantityUOM.Id;
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
                    throw new KnownException("Collected Quantity UOM is required");
                }
                else
                {
                    dbModel.CollectedQuantityUOM = model.CollectedQuantityUOM.Id;
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
                    dbModel.DeliveredQuantityUOM = model.DeliveredQuantityUOM.Id;
                }
            }
            SetBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}