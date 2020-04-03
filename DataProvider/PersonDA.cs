using Catalogs;
using Models;
using Models.BriefModel;
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
            SetBaseProperties(dbModel, model);
            if (dbModel.Id == 0)
            {
                dbModel.AuthUserId = model.AuthUserId;
            }
            return dbModel;
        }

        public async Task<List<PersonBriefModel>> GetPersonForDD(string filter)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from p in context.People
                              join a in context.Addresses on p.Id equals a.EntityId
                              where
                              (p.Name.Contains(filter) || p.NativeName.Contains(filter)
                              || p.IdentificationNo.Contains(filter) || a.MobileNo.Contains(filter)
                              )
                              && a.Type == (int)AddressTypeCatalog.Default
                              && a.EntityType == (int)EntityTypeCatalog.Person
                              select new PersonBriefModel()
                              {
                                  Id = p.Id,
                                  Name = p.Name,
                                  NativeName = p.NativeName,
                                  IdentificationNo = p.IdentificationNo,
                                  MobileNo = a.MobileNo,
                              }).ToListAsync();
            }
        }
    }
}
