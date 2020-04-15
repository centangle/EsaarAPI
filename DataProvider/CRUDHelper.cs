using Helpers;
using Models.BriefModel;
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
        public void SetAndValidateBaseProperties<T, M>(T dbModel, M model)
            where T : IBase
            where M : IBase
        {

            if (dbModel.Id == 0)
            {
                dbModel.IsActive = true;
                dbModel.IsDeleted = false;
                dbModel.CreatedBy = _loggedInMemberId;
                dbModel.CreatedDate = model.CreatedDate;
            }
            else
            {
                dbModel.IsActive = model.IsActive;
                dbModel.IsDeleted = model.IsDeleted;
            }
            dbModel.UpdatedBy = _loggedInMemberId;
            dbModel.UpdatedDate = model.UpdatedDate;
            if (typeof(M) == typeof(IName))
            {
                VerifyNameIsNotNull(model as IName);
            }
        }
        public M SetEntityId<M>(M model, string errorMessage)
            where M : BaseBriefModel, new()
        {
            bool isModerator = true;
            if (model != null && model.Id > 0 && isModerator)
            {
                return model;
            }
            else
            {
                if (_loggedInMemberId == 0)
                    throw new KnownException(errorMessage);
                else
                    model = new M();
                model.Id = _loggedInMemberId;
                return model;
            }

        }

        public void VerifyNameIsNotNull(IName model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new KnownException("Name can not be null");
            }

        }
    }
}
