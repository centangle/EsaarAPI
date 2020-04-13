namespace Catalogs
{
    public enum StatusCatalog : int
    {
        Initiated = 0,
        ModeratorAssigned = 10,
        InProcess = 20,
        Approved = 30,
        VolunteerAssigned = 40,
        Collected = 50,
        Delivered = 60,
        Rejected = 100,
    }

}
