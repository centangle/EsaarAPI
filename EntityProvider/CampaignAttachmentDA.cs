using Catalogs;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<bool> AssignCampaignAttachments(int campaignId, List<AttachmentModel> attachments)
        {
            if (campaignId == 0)
            {
                throw new KnownException("Campaign is required");
            }
            if (attachments == null || attachments.Count == 0)
            {
                throw new KnownException("Attachments are required");
            }
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, campaignId, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                await AssignAttachments(_context, attachments, campaignId, Catalogs.AttachmentEntityTypeCatalog.Campaign, true);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new KnownException("You are not authorized to perform this action");

        }
        public async Task<bool> DeleteCampaignAttachment(int id)
        {
            var attachment = await _context.Attachments.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (attachment != null)
            {
                if (attachment.EntityType == (int)AttachmentEntityTypeCatalog.Campaign)
                {
                    var memberOrgRoles = (await GetMemberRoleForOrganization(_context, id, _loggedInMemberId)).FirstOrDefault();
                    if (IsOrganizationMemberModerator(memberOrgRoles))
                    {
                        attachment.IsDeleted = true;
                        return await _context.SaveChangesAsync() > 0;
                    }
                    else
                        throw new KnownException("You are not authorized to perform this action");
                }
            }
            return false;
        }
    }
}
