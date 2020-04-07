using Catalogs;
using Models.Base;
using Models.BriefModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    public class OrganizationMemberModel : BaseModel
    {
        public OrganizationMemberModel()
        {
            Organization = new BaseBriefModel();
            Member = new BaseBriefModel();
        }
        public BaseBriefModel Organization { get; set; }
        public BaseBriefModel Member { get; set; }
        public List<OrganizationMemberRolesCatalog> Roles { get; set; }
        public OrganizationMemberRolesCatalog Role { get; set; }
    }
    public class OrganizationMemberSearchModel : BaseSearchModel
    {
        public OrganizationMemberSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public string MemberName { get; set; }
        public string OrganizationName { get; set; }
        public int? OrganizationId { get; set; }
        public int? MemberId { get; set; }
        public OrganizationMemberRolesCatalog? Role { get; set; }

    }
}
