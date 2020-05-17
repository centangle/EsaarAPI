using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Catalogs
{
    public enum OrganizationMemberRolesCatalog : int
    {
        Owner,
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
        Owner,
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
