using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public partial class Logic
    {
        public async Task<int> RequestOrganizationMembership(OrganizationRequestModel model)
        {
            return await _dataAccess.RequestOrganizationMembership(model);
        }
    }
}
