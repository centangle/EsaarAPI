namespace Models.BriefModel
{
    public class BaseBriefModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
    }
    public class BaseImageBriefModel : BaseBriefModel
    {
        public string ImageUrl { get; set; }
    }
}
