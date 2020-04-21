using DataProvider.Helpers;
using Helpers;
using Models;
using Models.BriefModel;
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
        public async Task<int> CreateOrganizationAccount(OrganizationAccountModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetOrganizationAccount(new OrganizationAccount(), model);
                var organizationMember = (await GetMemberRoleForOrganization(context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(organizationMember))
                {
                    context.OrganizationAccounts.Add(dbModel);
                    await context.SaveChangesAsync();
                    model.Id = dbModel.Id;
                    return model.Id;
                }
                else
                    throw new KnownException("You are not authorized to perform this action");
            }
        }
        public async Task<bool> UpdateOrganizationAccount(OrganizationAccountModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                OrganizationAccount dbModel = await context.OrganizationAccounts.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetOrganizationAccount(dbModel, model);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<bool> DeleteOrganizationAccount(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = await context.OrganizationAccounts.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        public async Task<OrganizationAccountModel> GetOrganizationAccount(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from oa in context.OrganizationAccounts
                              join o in context.Organizations on oa.OrganizationId equals o.Id
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
        }
        public async Task<PaginatedResultModel<OrganizationAccountModel>> GetOrganizationAccounts(OrganizationAccountSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var orgAccountQueryable = (from oa in context.OrganizationAccounts
                                           join o in context.Organizations on oa.OrganizationId equals o.Id
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
