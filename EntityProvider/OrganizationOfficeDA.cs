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
        public async Task<int> CreateOrganizationOffice(OrganizationOfficeModel model)
        {
            var dbModel = SetOrganizationOffice(new OrganizationOffice(), model);
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                _context.OrganizationOffices.Add(dbModel);
                await _context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
            else
                throw new KnownException("You are not authorized to perform this action");
        }
        public async Task<bool> UpdateOrganizationOffice(OrganizationOfficeModel model)
        {
            var memberOrgRoles = (await GetMemberRoleForOrganization(_context, model.Organization.Id, _loggedInMemberId)).FirstOrDefault();
            if (IsOrganizationMemberModerator(memberOrgRoles))
            {
                OrganizationOffice dbModel = await _context.OrganizationOffices.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetOrganizationOffice(dbModel, model);
                    return await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            else
                throw new KnownException("You are not authorized to perform this action");

        }
        public async Task<bool> DeleteOrganizationOffice(int id)
        {
            var dbModel = await _context.OrganizationOffices.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (dbModel != null)
            {
                var memberOrgRoles = (await GetMemberRoleForOrganization(_context, dbModel.OrganizationId, _loggedInMemberId)).FirstOrDefault();
                if (IsOrganizationMemberModerator(memberOrgRoles))
                {
                    dbModel.IsDeleted = true;
                    return await _context.SaveChangesAsync() > 0;
                }
                else
                    throw new KnownException("You are not authorized to perform this action");
            }
            return false;
        }
        public async Task<OrganizationOfficeModel> GetOrganizationOffice(int id)
        {
            return await (from oo in _context.OrganizationOffices
                          join o in _context.Organizations on oo.OrganizationId equals o.Id
                          where oo.Id == id
                          && oo.IsDeleted == false
                          select new OrganizationOfficeModel
                          {
                              Id = oo.Id,
                              Name = oo.Name,
                              NativeName = oo.NativeName,
                              Organization = new BaseBriefModel
                              {
                                  Id = o.Id,
                                  Name = o.Name,
                                  NativeName = o.NativeName,
                              },
                              Address = oo.Address,
                              AddressLatLong = oo.AddressLatLong,
                              Description = oo.Description,
                          }).FirstOrDefaultAsync();
        }
        public async Task<PaginatedResultModel<OrganizationOfficeModel>> GetOrganizationOffices(OrganizationOfficeSearchModel filters)
        {
            var orgOfficeQueryable = (from oo in _context.OrganizationOffices
                                      join o in _context.Organizations on oo.OrganizationId equals o.Id
                                      where
                                      (
                                        string.IsNullOrEmpty(filters.Name)
                                        || oo.Name.Contains(filters.Name)
                                        || oo.NativeName.Contains(filters.Name)
                                      )
                                      && oo.OrganizationId == filters.OrganizationId
                                      && oo.IsDeleted == false
                                      select new OrganizationOfficeModel
                                      {
                                          Id = oo.Id,
                                          Name = oo.Name,
                                          NativeName = oo.NativeName,
                                          Address = oo.Address,
                                          AddressLatLong = oo.AddressLatLong,
                                          Organization = new BaseBriefModel
                                          {
                                              Id = o.Id,
                                              Name = o.Name,
                                              NativeName = o.NativeName,
                                          },
                                          Description = oo.Description,
                                      }).AsNoTracking().AsQueryable();
            return await orgOfficeQueryable.Paginate(filters);
        }
        private OrganizationOffice SetOrganizationOffice(OrganizationOffice dbModel, OrganizationOfficeModel model)
        {
            if (model.Organization == null || model.Organization.Id == 0)
            {
                throw new KnownException("Organization is required");
            }
            dbModel.OrganizationId = model.Organization.Id;
            dbModel.Name = model.Name;
            dbModel.NativeName = model.NativeName;
            dbModel.Address = model.Address;
            dbModel.AddressLatLong = model.AddressLatLong;
            dbModel.Description = model.Description;
            SetAndValidateBaseProperties(dbModel, model);
            return dbModel;
        }
    }
}
