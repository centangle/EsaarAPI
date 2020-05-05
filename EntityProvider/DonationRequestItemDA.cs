using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public void AddDonationRequestItems(CharityContext _context, IEnumerable<DonationRequestItemModel> donationRequestItems, int donationRequestId)
        {
            foreach (var item in donationRequestItems)
            {
                var dbModel = new DonationRequestItem();
                SetRequestItem(dbModel, item, donationRequestId);
                _context.DonationRequestItems.Add(dbModel);
            }
        }
        private async Task UpdateDonationRequestItems(CharityContext _context, IEnumerable<DonationRequestItemModel> requestItems, int donationRequestId)
        {
            var currentDonationRequestItems = await _context.DonationRequestItems.Where(x => x.DonationRequestId == donationRequestId && x.IsDeleted == false).ToListAsync();
            var deletedDonationRequestItems = currentDonationRequestItems.Where(cri => !requestItems.Any(nri => nri.Id == cri.Id));
            var newDonationRequestItems = requestItems.Where(x => x.Id == 0);
            var updatedDonationRequestItems = currentDonationRequestItems.Where(cri => requestItems.Any(nri => nri.Id == cri.Id));

            AddDonationRequestItems(_context, newDonationRequestItems, donationRequestId);
            foreach (var dbRequestItem in updatedDonationRequestItems)
            {
                var modelRequestItem = requestItems.Where(x => x.Id == dbRequestItem.Id).FirstOrDefault();
                if (modelRequestItem != null)
                    SetRequestItem(dbRequestItem, modelRequestItem, donationRequestId);
            }
            foreach (var requestItem in deletedDonationRequestItems)
            {
                requestItem.IsDeleted = true;
            }

        }
        public void SetRequestItem(DonationRequestItem dbModel, DonationRequestItemModel model, int donationRequestId)
        {
            SetEntityId(model.QuantityUOM, "Quantity Unit in required");
            SetEntityId(model.Item, "Item in required");
            dbModel.DonationRequestId = donationRequestId;
            dbModel.ItemId = model.Item.Id;
            dbModel.Quantity = model.Quantity;
            dbModel.QuantityUom = model.QuantityUOM.Id;
            dbModel.Note = model.Note;
            if (model.DueDate > System.DateTime.Now.AddYears(-1))
                dbModel.DueDate = model.DueDate;
            SetAndValidateBaseProperties(dbModel, model);
        }
    }
}
