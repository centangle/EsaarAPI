namespace Models.Interfaces
{
    public interface IBase
    {
        int Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
