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
        public async Task<int> CreateOrganizationOffice(OrganizationOfficeModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = SetOrganizationOffice(new OrganizationOffice(), model);
                context.OrganizationOffices.Add(dbModel);
                await context.SaveChangesAsync();
                model.Id = dbModel.Id;
                return model.Id;
            }
        }
        public async Task<bool> UpdateOrganizationOffice(OrganizationOfficeModel model)
        {
            using (CharityEntities context = new CharityEntities())
            {
                OrganizationOffice dbModel = await context.OrganizationOffices.Where(x => x.Id == model.Id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    SetOrganizationOffice(dbModel, model);
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }

        }
        public async Task<bool> DeleteOrganizationOffice(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var dbModel = await context.OrganizationOffices.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    return await context.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
        public async Task<OrganizationOfficeModel> GetOrganizationOffice(int id)
        {
            using (CharityEntities context = new CharityEntities())
            {
                return await (from oo in context.OrganizationOffices
                              join o in context.Organizations on oo.OrganizationId equals o.Id
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
        }
        public async Task<PaginatedResultModel<OrganizationOfficeModel>> GetOrganizationOffices(OrganizationOfficeSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var orgOfficeQueryable = (from oo in context.OrganizationOffices
                                          join o in context.Organizations on oo.OrganizationId equals o.Id
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
        }
        private OrganizationOffice SetOrganizationOffice(OrganizationOffice dbModel, OrganizationOfficeModel model)
        {
            if (model.Organization == null || model.Organization.Id == 0)
            {
                throw new KnownException("Organization is required");
            }
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
