using Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RegionSearchModel : BaseSearchModel
    {
        public RegionSearchModel()
        {
            OrderByColumn = "Name";
        }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
