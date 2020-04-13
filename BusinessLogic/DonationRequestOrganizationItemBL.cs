using Catalogs;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> UpdateDonationRequestStatus(int donationRequestOrganizationId, string note, List<DonationRequestOrganizationItemModel> items, StatusCatalog status)
        {
            return await _dataAccess.UpdateDonationRequestStatus(donationRequestOrganizationId, note, items, status);
        }
    }
}
