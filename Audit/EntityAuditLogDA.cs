using AuditProvider.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuditProvider
{
    public partial class Audit
    {
        public async Task CreateEntityAuditLog()
        {
            EntityAuditLog log = new EntityAuditLog();
            _context.EntityAuditLog.Add(log);
            _context.SaveChangesAsync();
        }
    }
}
