using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs
{
    public enum OrganizationMemberTypeCatalog : int
    {
        Member,
        Volunteer,
        Moderator
    }
    public enum OrganizationRequestEntityTypeCatalog : int
    {
        Member,
        Item,
        Region
    }
    public enum OrganizationRequestTypeCatalog : int
    {
        Member,
        Volunteer,
        Moderator,
        Item,
        Region,
    }
    public enum OrganizationTypeCatalog : int
    {
        General
    }
}
