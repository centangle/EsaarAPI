﻿using Catalogs;
using EntityProvider.Helpers;
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
        public async Task<PaginatedResultModel<RegionBriefModel>> GetCountries(RegionSearchModel filters)
        {
            var regionQueryable = (from c in _context.Countries
                                   where (
                                   string.IsNullOrEmpty(filters.Name)
                                   || (c.Name.ToLower().Contains(filters.Name.ToLower())
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
        public async Task<PaginatedResultModel<RegionBriefModel>> GetStates(RegionSearchModel filters)
        {
            var stateQueryable = (from s in _context.States
                                  where (
                                  (
                                  string.IsNullOrEmpty(filters.Name)
                                  || s.Name.ToLower().Contains(filters.Name.ToLower())
                                  || s.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                  && (filters.ParentId == null || s.CountryId == filters.ParentId)
                                  && s.IsDeleted == false)
                                  select s).AsQueryable();


            if (filters.OrganizationId != null && filters.OrganizationId > 0)
            {
                stateQueryable = (from s in stateQueryable
                                  from er in _context.EntityRegions.Where(x => x.StateId == null || x.StateId == s.Id)
                                  where er.EntityId == filters.OrganizationId
                                  && (er.CountryId == s.CountryId || er.CountryId == null)
                                  && er.EntityType == (int)EntityRegionTypeCatalog.Organization
                                  && er.IsActive == true
                                  && er.IsDeleted == false
                                  select s).AsQueryable();
            }
            var sql = stateQueryable.ToString();
            var regionQueryable = (from s in stateQueryable
                                   select new RegionBriefModel
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       NativeName = s.NativeName
                                   }).AsNoTracking().Distinct().AsQueryable();
            return await regionQueryable.Paginate(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetDistricts(RegionSearchModel filters)
        {
            var districtQueryable = (from d in _context.Districts
                                     where (
                                     (string.IsNullOrEmpty(filters.Name)
                                     || d.Name.ToLower().Contains(filters.Name.ToLower())
                                     || d.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                     && (filters.ParentId == null || d.StateId == filters.ParentId)
                                     && d.IsDeleted == false)
                                     select d).AsQueryable();
            if (filters.OrganizationId != null && filters.OrganizationId > 0)
            {
                districtQueryable = (from d in districtQueryable
                                     from er in _context.EntityRegions.Where(x => x.DistrictId == d.Id || x.DistrictId == null)
                                     where er.EntityId == filters.OrganizationId
                                     && (er.StateId == d.StateId || er.StateId == null)
                                     && er.EntityType == (int)EntityRegionTypeCatalog.Organization
                                     && er.IsActive == true
                                     && er.IsDeleted == false
                                     select d).AsQueryable();
            }
            var regionQueryable = (from d in districtQueryable
                                   select new RegionBriefModel
                                   {
                                       Id = d.Id,
                                       Name = d.Name,
                                       NativeName = d.NativeName
                                   }).AsNoTracking().Distinct().AsQueryable();
            var sql = regionQueryable.ToString();
            return await regionQueryable.Paginate(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetTehsils(RegionSearchModel filters)
        {
            var tehsilQueryable = (from t in _context.Tehsils
                                   where (
                                   (string.IsNullOrEmpty(filters.Name)
                                   || t.Name.ToLower().Contains(filters.Name.ToLower())
                                   || t.NativeName.ToLower().Contains(filters.Name.ToLower()))
                                   && (filters.ParentId == null || t.DistrictId == filters.ParentId)
                                   && t.IsDeleted == false)
                                   select t).AsQueryable();
            if (filters.OrganizationId != null && filters.OrganizationId > 0)
            {
                tehsilQueryable = (from t in tehsilQueryable
                                   from er in _context.EntityRegions.Where(x => x.TehsilId == null || x.TehsilId == t.Id)
                                   where er.EntityId == filters.OrganizationId
                                   && (t.DistrictId == er.DistrictId || er.DistrictId == null)
                                   && er.EntityType == (int)EntityRegionTypeCatalog.Organization
                                   && er.IsActive == true
                                   && er.IsDeleted == false
                                   select t).AsQueryable();
            }
            var regionQueryable = (from t in tehsilQueryable
                                   select new RegionBriefModel
                                   {
                                       Id = t.Id,
                                       Name = t.Name,
                                       NativeName = t.NativeName
                                   }).AsNoTracking().Distinct().AsQueryable();
            return await regionQueryable.Paginate(filters);
        }
        public async Task<PaginatedResultModel<RegionBriefModel>> GetUnionCouncils(RegionSearchModel filters)
        {
            var ucQueryable = (from uc in _context.UnionCouncils
                               where (
                               (string.IsNullOrEmpty(filters.Name)
                               || uc.Name.ToLower().Contains(filters.Name.ToLower())
                               || uc.NativeName.ToLower().Contains(filters.Name.ToLower()))
                               && (filters.ParentId == null || uc.TehsilId == filters.ParentId)
                               && uc.IsDeleted == false)
                               select uc).AsQueryable();
            if (filters.OrganizationId != null && filters.OrganizationId > 0)
            {
                ucQueryable = (from uc in ucQueryable
                               from er in _context.EntityRegions.Where(x => x.UnionCouncilId == null || x.UnionCouncilId == uc.Id)
                               where er.EntityId == filters.OrganizationId
                               && (uc.TehsilId == er.TehsilId || er.TehsilId == null)
                               && er.EntityType == (int)EntityRegionTypeCatalog.Organization
                               && er.IsActive == true
                               && er.IsDeleted == false
                               select uc).AsQueryable();
            }
            var regionQueryable = (from uc in ucQueryable
                                   select new RegionBriefModel
                                   {
                                       Id = uc.Id,
                                       Name = uc.Name,
                                       NativeName = uc.NativeName
                                   }).AsNoTracking().Distinct().AsQueryable();
            return await regionQueryable.Paginate(filters);

        }
        public async Task<RegionBriefModel> GetCountry(CharityContext _context, int id)
        {
            return await (from c in _context.Countries
                          where c.Id == id
                          && c.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = c.Id,
                              Name = c.Name,
                              NativeName = c.NativeName,
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetState(CharityContext _context, int id)
        {
            return await (from s in _context.States
                          where s.Id == id
                          && s.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = s.Id,
                              Name = s.Name,
                              NativeName = s.NativeName,
                              ParentId = s.CountryId,
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetDistrict(CharityContext _context, int id)
        {
            return await (from d in _context.Districts
                          where d.Id == id
                          && d.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = d.Id,
                              Name = d.Name,
                              NativeName = d.NativeName,
                              ParentId = d.StateId,
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetTehsil(CharityContext _context, int id)
        {
            return await (from t in _context.Tehsils
                          where t.Id == id
                          && t.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = t.Id,
                              Name = t.Name,
                              NativeName = t.NativeName,
                              ParentId = t.DistrictId,
                          }).FirstOrDefaultAsync();

        }
        public async Task<RegionBriefModel> GetUnionCouncil(CharityContext _context, int id)
        {
            return await (from uc in _context.UnionCouncils
                          where uc.Id == id
                          && uc.IsDeleted == false
                          select new RegionBriefModel
                          {
                              Id = uc.Id,
                              Name = uc.Name,
                              NativeName = uc.NativeName,
                              ParentId = uc.TehsilId
                          }).FirstOrDefaultAsync();

        }
    }
}
