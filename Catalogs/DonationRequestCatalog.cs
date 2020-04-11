namespace Catalogs
{
    public enum DonationRequestTypeCatalog : int
    {
        Beneficiary,
        Donor
    }
    public enum DonationRequestStatusCatalog : int
    {
        Initiated,
        InProcess,
        Approved,
        Rejected,
        Collected,
        Delivered,
    }
}
