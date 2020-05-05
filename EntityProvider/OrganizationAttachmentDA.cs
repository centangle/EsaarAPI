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
        public async Task<bool> AssignOrganizationAttachments(int organizationId, List<AttachmentModel> attachments)
        {
            if (organizationId == 0)
            {
                throw new KnownException("Organization is required");
            }
            if (attachments == null || attachments.Count == 0)
            {
                throw new KnownException("Attachments are required");
            }
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, organizationId, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                await AssignAttachments(_context, attachments, organizationId, Catalogs.AttachmentEntityTypeCatalog.Organization, true);
                return await _context.SaveChangesAsync() > 0;
            }
            else
                throw new KnownException("You are not authorized to perform this action");

        }
        public async Task<bool> DeleteOrganizationAttachment(int id)
        {
            var attachment = await _context.Attachments.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (attachment != null)
            {
                if (attachment.EntityType == (int)AttachmentEntityTypeCatalog.Organization)
                {
                    var memberOrgRoles = (await GetMemberRoleForOrganization(_context, id, _loggedInMemberId)).FirstOrDefault();
                    if (IsOrganizationMemberOwner(memberOrgRoles))
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
