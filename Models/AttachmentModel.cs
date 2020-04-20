using Catalogs;
using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class AttachmentModel : BaseModel
    {
        public AttachmentModel()
        {
            Entity = new BaseBriefModel();
        }
        public BaseBriefModel Entity { get; set; }
        public AttachmentEntityTypeCatalog EntityType
        {
            get; set;
        }
        public string Url { get; set; }
        public string OriginalFileName { get; set; }
        public string SystemFileName { get; set; }
        public string FileExtension { get; set; }
        public string Note { get; set; }
    }
    public class AttachmentSearchModel : BaseSearchModel
    {
        public AttachmentSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public int? EntityId { get; set; }
        public AttachmentEntityTypeCatalog? EntityType { get; set; }
    }
}
