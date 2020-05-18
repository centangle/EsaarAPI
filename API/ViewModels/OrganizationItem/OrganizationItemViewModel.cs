using API.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModels.OrganizationItem
{
    public class OrganizationItemViewModel
    {
        public int Id { get; set; }
        public BaseViewModel Item { get; set; }
        public double CampaignItemTarget { get; set; }
        public BaseViewModel CampaignItemUOM { get; set; }
    }
}
