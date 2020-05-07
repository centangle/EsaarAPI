using System;
using System.Collections.Generic;

namespace AuditProvider.DbModels
{
    public partial class AccessLog
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string IpAddress { get; set; }
        public int? UserId { get; set; }
        public DateTime Date { get; set; }
    }
}
