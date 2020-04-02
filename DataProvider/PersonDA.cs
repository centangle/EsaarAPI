using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DataProvider
{
    public partial class DataAccess
    {
        public async Task<PersonModel> GetPersonByAuthId(string AuthId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from p in context.People
                              where p.AuthUserId == AuthId
                              select new PersonModel()
                              {
                                  Id = p.Id
                              }).FirstOrDefaultAsync();
            }
        }
        public async Task<bool> AddPerson(PersonModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Person dbModel = new Person();
                        SetPerson(dbModel, model);
                        context.People.Add(dbModel);
                        bool result = await context.SaveChangesAsync() > 0;
                        model.Id = dbModel.Id;
                        model.Address.Entity.Id = model.Id;
                        await AddAddress(model.Address);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
        public async Task<bool> UpdatePerson(PersonModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                try
                {
                    Person dbModel = await context.People.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        SetPerson(dbModel, model);
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
        private Person SetPerson(Person dbModel, PersonModel model)
        {

            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Type = 0;
            dbModel.IdentificationNo = model.IdentificationNo;
            dbModel.IsActive = model.IsActive;
            dbModel.IsDeleted = model.IsDeleted;
            if (dbModel.Id == 0)
            {
                dbModel.CreatedBy = _currentPersonId;
                dbModel.CreatedDate = model.CreatedDate;
                dbModel.IsDeleted = false;
                dbModel.AuthUserId = model.AuthUserId;

            }
            dbModel.UpdatedBy = _currentPersonId;
            dbModel.UpdatedDate = model.UpdatedDate;
            return dbModel;
        }

        public async Task<List<BriefModel>> GetPersonForDD(string name)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from p in context.People
                              where p.Name.Contains(name) || p.NativeName.Contains(name)
                              select new BriefModel()
                              {
                                  Id = p.Id,
                                  Name = p.Name,
                                  NativeName = p.NativeName
                              }).ToListAsync();
            }
        }
    }
}
