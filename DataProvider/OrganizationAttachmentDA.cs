using Helpers;
using Models;
using System;
using System.Collections.Generic;
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
            if(attachments==null|| attachments.Count==0)
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
    }
}
