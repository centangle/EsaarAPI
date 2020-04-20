using Models.Base;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrganizationAttachmentModel : BaseModel
    {
        public BaseBriefModel Organization { get; set; }
        public List<AttachmentModel> Attachments { get; set; }
    }
}
