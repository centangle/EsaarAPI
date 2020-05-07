using AuditProvider.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuditProvider
{
    public partial class Audit
    {
        public async Task CreateAccessLog()
        {
            AccessLog log = new AccessLog();
            _context.AccessLog.Add(log);
            _context.SaveChangesAsync();
        }
    }
}
