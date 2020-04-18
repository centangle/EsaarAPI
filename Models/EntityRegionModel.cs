﻿using Catalogs;
using Models.Base;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EntityRegionModel : BaseModel
    {
        public BaseBriefModel Entity { get; set; }
        public EntityRegionTypeCatalog EntityType { get; set; }
        public RegionLevelTypeCatalog RegionLevel { get; set; }
        public RegionBriefModel Region { get; set; }
        public bool IsApproved { get; set; } = true;
    }
    public class PaginatedEntityRegionModel : EntityRegionModel
    {
    }
    public class EntityRegionSearchModel : BaseSearchModel
    {
        public EntityRegionSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public int EntityId { get; set; }
        public EntityRegionTypeCatalog EntityType { get; set; }

    }
}