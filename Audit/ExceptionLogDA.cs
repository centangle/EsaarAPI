using AuditProvider.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuditProvider
{
    public partial class Audit
    {
        public async Task CreateExceptionLog()
        {
            ExceptionLog log = new ExceptionLog();
            _context.ExceptionLog.Add(log);
            _context.SaveChangesAsync();
        }
    }
}
