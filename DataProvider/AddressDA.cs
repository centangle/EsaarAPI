using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<bool> AddAddress(AddressModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                Address dbModel = new Address();
                SetAddress(dbModel, model);
                context.Addresses.Add(dbModel);
                bool result = await context.SaveChangesAsync() > 0;
                model.Id = dbModel.Id;
                return result;
            }
        }
        public async Task<bool> UpdateAddress(AddressModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                try
                {
                    Address dbModel = await context.Addresses.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        SetAddress(dbModel, model);
                        return await context.SaveChangesAsync() > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private Address SetAddress(Address dbModel, AddressModel model)
        {
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.EntityId = model.Entity.Id;
            dbModel.EntityType = (int)model.EntityType;
            dbModel.Type = (int)model.Type;
            dbModel.AddressLine1 = model.AddressLine1;
            dbModel.AddressLine2 = model.AddressLine2;
            dbModel.ZipCode = model.ZipCode;
            dbModel.PhoneNo = model.PhoneNo;
            dbModel.MobileNo = model.MobileNo;
            dbModel.Email = model.Email;
            if (model.Country == null || model.Country.Id == 0)
            {
                dbModel.CountryId = null;
            }
            else
            {
                dbModel.CountryId = model.Country.Id;
            }
            if (model.State == null || model.State.Id == 0)
            {
                dbModel.StateId = null;
            }
            else
            {
                dbModel.StateId = model.State.Id;
            }
            if (model.District == null || model.District.Id == 0)
            {
                dbModel.DistrictId = null;
            }
            else
            {
                dbModel.DistrictId = model.District.Id;
            }
            if (model.Tehsil == null || model.Tehsil.Id == 0)
            {
                dbModel.TehsilId = null;
            }
            else
            {
                dbModel.TehsilId = model.Tehsil.Id;
            }
            if (model.UnionCouncil == null || model.UnionCouncil.Id == 0)
            {
                dbModel.UnionCouncilId = null;
            }
            else
            {
                dbModel.UnionCouncilId = model.UnionCouncil.Id;
            }
            dbModel.IsActive = model.IsActive;
            dbModel.IsDeleted = model.IsDeleted;
            if (dbModel.Id == 0)
            {
                dbModel.CreatedBy = model.CreatedBy;
                dbModel.CreatedDate = model.CreatedDate;
                dbModel.IsDeleted = false;

            }
            dbModel.UpdatedBy = model.UpdatedBy;
            dbModel.UpdatedDate = model.UpdatedDate;
            return dbModel;
        }
    }
}
