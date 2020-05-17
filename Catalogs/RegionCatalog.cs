using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogs
{
    public enum RegionLevelTypeCatalog : int
    {
        Country,
        State,
        District,
        Tehsil,
        UnionCouncil
    }
    public enum RegionRadiusTypeCatalog : int
    {
        Meters,
        Kilometers
    }
    public enum RegionSearchMethodCatalog : int
    {
        Contains,
        Intersects
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegionSearchTypeCatalog : int
    {
        InMyRegion,
        InRadius,
        ByRegion

    }
}
