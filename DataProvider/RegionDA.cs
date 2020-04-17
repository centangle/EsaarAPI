using DataProvider.Helpers;
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
        public async Task<PaginatedResultModel<RegionBriefModel>> GetCountries(RegionSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var regionQueryable = (from c in context.Countries
                                       where (
                                       (c.Name.ToLower().Contains(filters.Name.ToLower())
                                       || c.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                       && c.IsDeleted == false)
                                       select new RegionBriefModel
                                       {
                                           Id = c.Id,
                                           Name = c.Name,
                                           NativeName = c.NativeName
                                       }).AsNoTracking().AsQueryable();
                return await regionQueryable.Paginate(filters);
            }

        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetStates(RegionSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var regionQueryable = (from s in context.States
                                       where (
                                       (s.Name.ToLower().Contains(filters.Name.ToLower())
                                       || s.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                       && (filters.ParentId == null || s.CountryId == filters.ParentId)
                                       && s.IsDeleted == false)
                                       select new RegionBriefModel
                                       {
                                           Id = s.Id,
                                           Name = s.Name,
                                           NativeName = s.NativeName
                                       }).AsNoTracking().AsQueryable();
                return await regionQueryable.Paginate(filters);
            }

        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetDistricts(RegionSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var regionQueryable = (from d in context.Districts
                                       where (
                                       (d.Name.ToLower().Contains(filters.Name.ToLower())
                                       || d.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                       && (filters.ParentId == null || d.StateId == filters.ParentId)
                                       && d.IsDeleted == false)
                                       select new RegionBriefModel
                                       {
                                           Id = d.Id,
                                           Name = d.Name,
                                           NativeName = d.NativeName
                                       }).AsNoTracking().AsQueryable();
                return await regionQueryable.Paginate(filters);
            }

        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetTehsils(RegionSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var regionQueryable = (from t in context.Tehsils
                                       where (
                                       (t.Name.ToLower().Contains(filters.Name.ToLower())
                                       || t.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                       && (filters.ParentId == null || t.DistrictId == filters.ParentId)
                                       && t.IsDeleted == false)
                                       select new RegionBriefModel
                                       {
                                           Id = t.Id,
                                           Name = t.Name,
                                           NativeName = t.NativeName
                                       }).AsNoTracking().AsQueryable();
                return await regionQueryable.Paginate(filters);
            }

        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetUnionCouncils(RegionSearchModel filters)
        {
            using (CharityEntities context = new CharityEntities())
            {
                var regionQueryable = (from u in context.UnionCouncils
                                       where (
                                       (u.Name.ToLower().Contains(filters.Name.ToLower())
                                       || u.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                       && (filters.ParentId == null || u.TehsilId == filters.ParentId)
                                       && u.IsDeleted == false)
                                       select new RegionBriefModel
                                       {
                                           Id = u.Id,
                                           Name = u.Name,
                                           NativeName = u.NativeName
                                       }).AsNoTracking().AsQueryable();
                return await regionQueryable.Paginate(filters);
            }

        }
        public async Task<RegionBriefModel> GetCountry(CharityEntities context, int id)
        {
            return await (from c in context.Countries
                          where c.Id == id
                          && c.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = c.Id,
                              Name = c.Name,
                              NativeName = c.NativeName
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetState(CharityEntities context, int id)
        {
            return await (from s in context.States
                          where s.Id == id
                          && s.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = s.Id,
                              Name = s.Name,
                              NativeName = s.NativeName
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetDistrict(CharityEntities context, int id)
        {
            return await (from d in context.Districts
                          where d.Id == id
                          && d.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = d.Id,
                              Name = d.Name,
                              NativeName = d.NativeName
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetTehsil(CharityEntities context, int id)
        {
            return await (from t in context.Tehsils
                          where t.Id == id
                          && t.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = t.Id,
                              Name = t.Name,
                              NativeName = t.NativeName
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetUnionCouncil(CharityEntities context, int id)
        {
            return await (from uc in context.UnionCouncils
                          where uc.Id == id
                          && uc.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = uc.Id,
                              Name = uc.Name,
                              NativeName = uc.NativeName
                          }).FirstOrDefaultAsync();

        }
    }
}
