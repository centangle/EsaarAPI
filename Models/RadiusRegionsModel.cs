using Catalogs;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IRadiusRegionSearch
    {
         double Longitude { get; set; }
         double Latitude { get; set; }
         float? Radius { get; set; }
         List<int> RootCategories { get; set; }
         int? RegionId { get; set; }
         RegionSearchTypeCatalog? SearchType { get; set; }
         List<RegionLevelSearchModel> Regions { get; set; }
         RegionRadiusTypeCatalog? RadiusType { get; set; }
    }
    public class FilteredRegionsModel
    {
        public FilteredRegionsModel()
        {
            Countries = new List<BaseBriefModel>();
            States = new List<BaseBriefModel>();
            Districts = new List<BaseBriefModel>();
            Tehsils = new List<BaseBriefModel>();
            UnionCouncils = new List<BaseBriefModel>();
        }
        public List<BaseBriefModel> Countries { get; set; }
        public List<BaseBriefModel> States { get; set; }
        public List<BaseBriefModel> Districts { get; set; }
        public List<BaseBriefModel> Tehsils { get; set; }
        public List<BaseBriefModel> UnionCouncils { get; set; }
    }
}
