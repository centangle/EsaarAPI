using Catalogs;
using Models.Base;
using Models.BriefModel;
using System.Collections.Generic;

namespace Models
{
    public class RequestThreadModel : BaseModel
    {
        public RequestThreadModel()
        {
            Entity = new BaseBriefModel();
            Creator = new BaseBriefModel();
        }
        public BaseBriefModel Creator { get; set; }
        public BaseBriefModel Entity { get; set; }
        public RequestThreadEntityTypeCatalog EntityType { get; set; } = RequestThreadEntityTypeCatalog.Donation;
        public StatusCatalog? Status { get; set; }
        public RequestThreadTypeCatalog Type { get; set; } = RequestThreadTypeCatalog.General;
        public List<AttachmentModel> Attachments { get; set; }
        public bool IsSystemGenerated { get; set; }
        public string Note { get; set; }
    }
    public class RequestThreadSearchModel : BaseSearchModel
    {
        public RequestThreadSearchModel()
        {
            OrderByColumn = "CreatedDate";
        }
        public int EntityId { get; set; }
        public RequestThreadEntityTypeCatalog EntityType { get; set; }

        public RequestThreadTypeCatalog Type { get; set; }
    }
}
