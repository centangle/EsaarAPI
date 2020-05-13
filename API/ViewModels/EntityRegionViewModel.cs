using Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.ViewModels
{
    public class EntityRegionViewModel
    {
        public int RegionId { get; set; }
        public RegionLevelTypeCatalog RegionLevel { get; set; }
    }
}
