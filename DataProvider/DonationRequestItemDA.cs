using Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public void AddDonationRequestItems(CharityEntities context, IEnumerable<DonationRequestItemModel> donationRequestItems, int donationRequestId)
        {
            foreach (var item in donationRequestItems)
            {
                var dbModel = new DonationRequestItem();
                item.DonationRequestId = donationRequestId;
                SetRequestItem(dbModel, item);
                context.DonationRequestItems.Add(dbModel);
            }
        }
        private async Task UpdateDonationRequestItems(CharityEntities context, IEnumerable<DonationRequestItemModel> requestItems, int requestId)
        {

            var currentDonationRequestItems = await context.DonationRequestItems.Where(x => x.DonationRequestId == requestId && x.IsDeleted == false).ToListAsync();
            var deletedDonationRequestItems = currentDonationRequestItems.Where(cri => !requestItems.Any(nri => nri.Id == cri.Id));
            var newDonationRequestItems = requestItems.Where(x => x.Id == 0);
            var updatedDonationRequestItems = currentDonationRequestItems.Where(cri => requestItems.Any(nri => nri.Id == cri.Id));

            AddDonationRequestItems(context, newDonationRequestItems, requestId);
            foreach (var dbRequestItem in updatedDonationRequestItems)
            {
                var modelRequestItem = requestItems.Where(x => x.Id == dbRequestItem.Id).FirstOrDefault();
                if (modelRequestItem != null)
                    SetRequestItem(dbRequestItem, modelRequestItem);
            }
            foreach (var requestItem in deletedDonationRequestItems)
            {
                requestItem.IsDeleted = true;
            }

        }
        public void SetRequestItem(DonationRequestItem dbModel, DonationRequestItemModel model)
        {
            SetEntityId(model.SelectedUnit, "Selected Unit in required");
            SetEntityId(model.Item, "Item in required");
            dbModel.DonationRequestId = model.DonationRequestId;
            dbModel.ItemId = model.Item.Id;
            dbModel.Quantity = model.Quantity;
            dbModel.SelectedUnit = model.SelectedUnit.Id;
            dbModel.Note = model.Note;
            if (model.DueDate > System.DateTime.Now.AddYears(-1))
                dbModel.DueDate = model.DueDate;
            SetBaseProperties(dbModel, model);
        }
    }
}
