using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
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
