using System;
using System.Collections.Generic;

namespace AuditProvider.DbModels
{
    public partial class EntityAuditDetail
    {
        public int Id { get; set; }
        public Guid Identifier { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
    }
}
