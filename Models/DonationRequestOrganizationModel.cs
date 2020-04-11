using Catalogs;
using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class DonationRequestOrganizationModel : BaseModel
    {
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel AssignedTo { get; set; }
        public DonationRequestStatusCatalog? Status { get; set; }
    }
}
