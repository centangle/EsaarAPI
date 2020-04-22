using Catalogs;
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
        public EntityRegionModel()
        {
            Entity = new BaseBriefModel();
            Region = new RegionBriefModel();
        }
        public BaseBriefModel Entity { get; set; }
        public EntityRegionTypeCatalog EntityType { get; set; }
        public RegionLevelTypeCatalog RegionLevel { get; set; }
        public RegionBriefModel Region { get; set; }
        public int? RequestId { get; set; }
        public EntityRegionRequestTypeCatalog RequestType { get; set; }
        public bool IsApproved { get; set; } = true;
        public BaseBriefModel Country { get; set; }
        public BaseBriefModel State { get; set; }
        public BaseBriefModel District { get; set; }
        public BaseBriefModel Tehsil { get; set; }
        public BaseBriefModel UnionCouncil { get; set; }
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
