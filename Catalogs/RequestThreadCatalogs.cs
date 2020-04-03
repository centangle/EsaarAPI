namespace Catalogs
{
    public enum RequestThreadTypeCatalog : int
    {
        General, // Entity Type Already has Source of Request
    }
    public enum RequestThreadEntityTypeCatalog : int
    {
        Request,
        Organization,
        Complain,

    }

    public enum RequestThreadStatusCatalog : int
    {
        Initiated,
        InProcess,
        Approved,
        Rejected
    }
}
