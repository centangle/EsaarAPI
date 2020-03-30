namespace Models.Interfaces
{
    public interface IImage
    {
        string BaseFolder { get; }
        string ImageUrl { get; set; }
        string ImageInBase64 { get; set; }
    }
}
