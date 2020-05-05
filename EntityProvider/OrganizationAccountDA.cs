using EntityProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityProvider.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<int> CreateOrganizationAccount(OrganizationAccountModel model)
        {
            var dbModel = SetOrganizationAccount(new OrganizationAccount(), model);
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                _context.OrganizationAccounts.Add(dbModel);
                await _context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
            else
                throw new KnownException("You are not authorized to perform this action");
        }
        public async Task<bool> UpdateOrganizationAccount(OrganizationAccountModel model)
        {
            OrganizationAccount dbModel = await _context.OrganizationAccounts.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                if (dbModel != null)
                {
                    SetOrganizationAccount(dbModel, model);
                    return await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            else
                throw new KnownException("You are not authorized to perform this action");

        }
        public async Task<bool> DeleteOrganizationAccount(int id)
        {
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                var dbModel = await _context.OrganizationAccounts.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            else
                throw new KnownException("You are not authorized to perform this action");
        }
        public async Task<OrganizationAccountModel> GetOrganizationAccount(int id)
        {
            return await (from oa in _context.OrganizationAccounts
                          join o in _context.Organizations on oa.OrganizationId equals o.Id
                          where oa.Id == id
                          && oa.IsDeleted == false
                          select new OrganizationAccountModel
                          {
                              Id = oa.Id,
                              Name = oa.Name,
                              NativeName = oa.NativeName,
                              Organization = new BaseBriefModel
                              {
                                  Id = o.Id,
                                  Name = o.Name,
                                  NativeName = o.NativeName,
                              },
                              AccountNo = oa.AccountNo,
                              Description = oa.Description,
                          }).FirstOrDefaultAsync();
        }
        public async Task<PaginatedResultModel<OrganizationAccountModel>> GetOrganizationAccounts(OrganizationAccountSearchModel filters)
        {
            var orgAccountQueryable = (from oa in _context.OrganizationAccounts
                                       join o in _context.Organizations on oa.OrganizationId equals o.Id
                                       where
                                       (
                                         string.IsNullOrEmpty(filters.Name)
                                         || oa.Name.Contains(filters.Name)
                                         || oa.NativeName.Contains(filters.Name)
                                       )
                                       && oa.OrganizationId == filters.OrganizationId
                                       && oa.IsDeleted == false
                                       select new OrganizationAccountModel
                                       {
                                           Id = oa.Id,
                                           Name = oa.Name,
                                           NativeName = oa.NativeName,
                                           AccountNo = oa.AccountNo,
                                           Organization = new BaseBriefModel
                                           {
                                               Id = o.Id,
                                               Name = o.Name,
                                               NativeName = o.NativeName,
                                           },
                                           Description = oa.Description,
                                       }).AsNoTracking().AsQueryable();
            return await orgAccountQueryable.Paginate(filters);
        }
        private OrganizationAccount SetOrganizationAccount(OrganizationAccount dbModel, OrganizationAccountModel model)
        {
            if (model.Organization == null || model.Organization.Id == 0)
            {
                throw new KnownException("Organization is required");
            }
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.AccountNo = model.AccountNo;
            dbModel.Description = model.Description;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}
