using Catalogs;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class DonationRequestThreadModel
    {
        public int DonationRequestOrganizationId { get; set; }
        public List<DonationRequestOrganizationItemModel> Items { get; set; }
        public StatusCatalog? Status { get; set; }
        public string Note { get; set; }
        public BaseBriefModel Moderator { get; set; }
        public BaseBriefModel Volunteer { get; set; }
    }
}
