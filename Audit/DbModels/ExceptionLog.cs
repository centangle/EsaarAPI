using System;
using System.Collections.Generic;

namespace AuditProvider.DbModels
{
    public partial class ExceptionLog
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string MethodName { get; set; }
        public string Exception { get; set; }
        public string SerializedModel { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
