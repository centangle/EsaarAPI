using AuditProvider.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuditProvider
{
    public partial class Audit
    {
        public async Task CreateEntityAuditDetail()
        {
            EntityAuditDetail log = new EntityAuditDetail();
            _context.EntityAuditDetail.Add(log);
            _context.SaveChangesAsync();
        }
    }
}
