using Catalogs;
using Models.Base;
using Models.BriefModel;

namespace Models
{
    public class RequestThreadModel : BaseModel
    {
        public RequestThreadModel()
        {
            Entity = new BaseBriefModel();
        }
        public BaseBriefModel Entity { get; set; }
        public RequestThreadEntityTypeCatalog EntityType { get; set; } = RequestThreadEntityTypeCatalog.Request;
        public RequestThreadStatusCatalog Status { get; set; } = RequestThreadStatusCatalog.Initiated;
        public RequestThreadTypeCatalog Type { get; set; } = RequestThreadTypeCatalog.General;
        public string Note { get; set; }
    }
}
