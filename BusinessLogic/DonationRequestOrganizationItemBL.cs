using Models;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<bool> UpdateDonationRequestStatus(DonationRequestThreadModel model)
        {
            return await _dataAccess.UpdateDonationRequestStatus(model);
        }
    }
}
