using Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BriefModel
{
    public class EntityRegionBriefModel
    {
        public RegionLevelTypeCatalog RegionLevel { get; set; }
        public RegionBriefModel Region { get; set; }
        public BaseBriefModel Country { get; set; }
        public BaseBriefModel State { get; set; }
        public BaseBriefModel District { get; set; }
        public BaseBriefModel Tehsil { get; set; }
        public BaseBriefModel UnionCouncil { get; set; }
    }
}
