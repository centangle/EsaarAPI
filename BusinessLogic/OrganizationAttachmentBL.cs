﻿using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {

        public async Task<bool> AssignOrganizationAttachments(int organizationId, List<AttachmentModel> attachments)
        {
            return await _dataAccess.AssignOrganizationAttachments(organizationId, attachments);
        }
        public async Task<bool> DeleteOrganizationAttachment(int id)
        {
            return await _dataAccess.DeleteOrganizationAttachment(id);
        }
    }
}
