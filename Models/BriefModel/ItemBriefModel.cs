using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BriefModel
{
    public class ItemBriefModel : BaseBriefModel
    {
        public string ImageUrl { get; set; }
        public double Worth { get; set; }
        public string Description { get; set; }
    }
}
