using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModels.Campaign
{
    public class CampaignRegionViewModel
    {
        public CampaignRegionViewModel()
        {
            Regions = new List<EntityRegionViewModel>();
        }
        public int CampaignId { get; set; }
        public List<EntityRegionViewModel> Regions { get; set; }
    }
}
