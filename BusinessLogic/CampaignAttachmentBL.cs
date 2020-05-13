using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {

        public async Task<bool> AssignCampaignAttachments(int campaignId, List<AttachmentModel> attachments)
        {
            return await _dataAccess.AssignCampaignAttachments(campaignId, attachments);
        }
        public async Task<bool> DeleteCampaignAttachment(int id)
        {
            return await _dataAccess.DeleteCampaignAttachment(id);
        }
    }
}
