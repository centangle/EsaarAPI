using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> AddDonationRequestOrganizationItems(List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            return await _dataAccess.AddDonationRequestOrganizationItems(items, donationRequestOrganizationId);
        }
        public async Task<bool> UpdateDonationRequestOrganizationItems(List<DonationRequestOrganizationItemModel> items, int donationRequestOrganizationId)
        {
            return await _dataAccess.UpdateDonationRequestOrganizationItems(items, donationRequestOrganizationId);
        }
    }
}
