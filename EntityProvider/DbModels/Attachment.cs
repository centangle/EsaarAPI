using System;
using System.Collections.Generic;

namespace EntityProvider.DbModels
{
    public partial class Attachment
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int EntityType { get; set; }
        public string Url { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public string OriginalFileName { get; set; }
        public string SystemFileName { get; set; }
        public string FileExtension { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
