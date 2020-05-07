using System;
using System.Collections.Generic;

namespace AuditProvider.DbModels
{
    public partial class EntityAuditLog
    {
        public int Id { get; set; }
        public string Pk { get; set; }
        public string TableName { get; set; }
        public string SerializedModel { get; set; }
        public int? ActionBy { get; set; }
        public DateTime? ActionDate { get; set; }
    }
}
