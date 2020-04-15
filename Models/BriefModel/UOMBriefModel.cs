namespace Models.BriefModel
{
    public class UOMBriefModel : BaseBriefModel
    {
        public double NoOfBaseUnit { get; set; }
    }
    public class UOMBriefParentModel : BaseBriefModel
    {
        public int? ParentId { get; set; }
    }
}
