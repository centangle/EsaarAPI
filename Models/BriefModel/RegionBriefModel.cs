using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models.BriefModel
{
    public class RegionBriefModel : BaseBriefModel
    {
        [IgnoreDataMember]
        public int ParentId { get; set; }
    }
}
