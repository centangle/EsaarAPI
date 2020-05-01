using Catalogs;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
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
            using (CharityEntities context = new CharityEntities())
            {
                var memberOrgRoles = (await GetMemberRoleForOrganization(context, organizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(memberOrgRoles))
                {
                    await AssignAttachments(context, attachments, organizationId, Catalogs.AttachmentEntityTypeCatalog.Organization, true);
                    return await context.SaveChangesAsync() > 0;
                }
                else
                    throw new KnownException("You are not authorized to perform this action");

            }
        }
        public async Task<bool> DeleteOrganizationAttachment(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var attachment = await context.Attachments.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (attachment != null)
                {
                    if (attachment.EntityType == (int)AttachmentEntityTypeCatalog.Organization)
                    {
                        var memberOrgRoles = (await GetMemberRoleForOrganization(context, id, _loggedInMemberId)).FirstOrDefault();
                        if (IsOrganizationMemberOwner(memberOrgRoles))
                        {
                            attachment.IsDeleted = true;
                            return await context.SaveChangesAsync() > 0;
                        }
                        else
                            throw new KnownException("You are not authorized to perform this action");
                    }
                }
                return false;
            }
        }
    }
}
