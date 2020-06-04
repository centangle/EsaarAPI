using Catalogs;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class OrganizationRequestThreadModel
    {
        public int OrganizationRequestId { get; set; }
        public StatusCatalog? Status { get; set; }
        public string Note { get; set; }
        public BaseBriefModel Moderator { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
    }
}
