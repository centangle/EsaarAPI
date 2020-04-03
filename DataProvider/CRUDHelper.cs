using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public void SetBaseProperties<T, M>(T dbModel, M model)
            where T : IBase
            where M : IBase
        {
            dbModel.IsActive = model.IsActive;
            if (dbModel.Id == 0)
            {
                dbModel.IsDeleted = false;
                dbModel.CreatedBy = _loggedInMemberId;
                dbModel.CreatedDate = model.CreatedDate;
            }
            else
            {
                dbModel.IsDeleted = model.IsDeleted;
            }
            dbModel.UpdatedBy = _loggedInMemberId;
            dbModel.UpdatedDate = model.UpdatedDate;
        }
    }
}
