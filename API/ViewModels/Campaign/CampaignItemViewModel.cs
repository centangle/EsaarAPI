using API.ViewModels.OrganizationItem;
using System.Collections.Generic;

namespace API.ViewModels.Campaign
{
    public class CampaignItemViewModel
    {
        public CampaignItemViewModel()
        {
            Items = new List<OrganizationItemViewModel>();
        }
        public int CampaignId { get; set; }
        public List<OrganizationItemViewModel> Items { get; set; }
    }
}
