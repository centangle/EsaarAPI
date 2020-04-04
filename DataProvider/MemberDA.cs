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
        public async Task<MemberModel> GetMemberByAuthId(string AuthId)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from m in context.Members
                              where m.AuthUserId == AuthId
                              select new MemberModel()
                              {
                                  Id = m.Id
                              }).FirstOrDefaultAsync();
            }
        }
        public async Task<bool> AddMember(MemberModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Member dbModel = new Member();
                        SetMember(dbModel, model);
                        context.Members.Add(dbModel);
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
        public async Task<bool> UpdateMember(MemberModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                try
                {
                    Member dbModel = await context.Members.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                    if (dbModel != null)
                    {
                        SetMember(dbModel, model);
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
        private Member SetMember(Member dbModel, MemberModel model)
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
        public async Task<List<MemberBriefModel>> GetMemberForDD(string filter)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from m in context.Members
                              join a in context.Addresses on m.Id equals a.EntityId
                              where
                              (m.Name.Contains(filter) || m.NativeName.Contains(filter)
                              || m.IdentificationNo.Contains(filter) || a.MobileNo.Contains(filter)
                              )
                              && a.Type == (int)AddressTypeCatalog.Default
                              && a.EntityType == (int)MemberTypeCatalog.Member
                              select new MemberBriefModel()
                              {
                                  Id = m.Id,
                                  Name = m.Name,
                                  NativeName = m.NativeName,
                                  IdentificationNo = m.IdentificationNo,
                                  MobileNo = a.MobileNo,
                              }).ToListAsync();
            }
        }
    }
}
