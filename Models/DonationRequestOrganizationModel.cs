using Catalogs;
using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class DonationRequestOrganizationModel : BaseModel
    {
        public DonationRequestOrganizationModel()
        {
            Organization = new BaseBriefModel();
            Moderator = new BaseBriefModel();
            Volunteer = new BaseBriefModel();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Moderator { get; set; }
        public BaseBriefModel Volunteer { get; set; }
        public StatusCatalog? Status { get; set; }
    }
}
