using Catalogs;
using Models;
using Models.BriefModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<MemberModel> GetMemberByAuthId(string AuthId)
        {
            return await (from m in _context.Members
                          where m.AuthUserId == AuthId
                          select new MemberModel()
                          {
                              Id = m.Id
                          }).FirstOrDefaultAsync();
        }
        public async Task<bool> AddMember(MemberModel model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Member dbModel = new Member();
                    SetMember(dbModel, model);
                    _context.Members.Add(dbModel);
                    bool result = await _context.SaveChangesAsync() > 0;
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
        public async Task<bool> UpdateMember(MemberModel model)
        {
            try
            {
                Member dbModel = await _context.Members.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetMember(dbModel, model);
                    return await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private Member SetMember(Member dbModel, MemberModel model)
        {

            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Type = 0;
            dbModel.IdentificationNo = model.IdentificationNo;
            SetAndValidateBaseProperties(dbModel, model);
            if (dbModel.Id == 0)
            {
                dbModel.AuthUserId = model.AuthUserId;
            }
            return dbModel;
        }
        public async Task<List<MemberBriefModel>> GetMemberForDD(string filter)
        {
            return await (from m in _context.Members
                          join a in _context.Addresses on m.Id equals a.EntityId
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
