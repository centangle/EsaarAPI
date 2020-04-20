using Models.Base;
using Models.BriefModel;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrganizationOfficeModel : BaseModel, IName
    {
        public BaseBriefModel Organization { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Address { get; set; }
        public string AddressLatLong { get; set; }
        public string Description { get; set; }
    }

    public class OrganizationOfficeSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public int OrganizationId { get; set; }
    }
}
