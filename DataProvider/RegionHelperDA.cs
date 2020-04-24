using Catalogs;
using Microsoft.SqlServer.Types;
using Models;
using Models.BriefModel;
using SqlServerTypes;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Dapper;

namespace DataProvider
{
    public partial class DataAccess
    {
        private SqlGeography GetRegionRadius(double latitude, double longitude, float radius, RegionRadiusTypeCatalog radiusType)
        {
            Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            var radiusInMeters = radius;
            if (radiusType == RegionRadiusTypeCatalog.Kilometers)
            {
                radiusInMeters = radius * 1000;
            }
            var point = SqlGeography.Point(latitude, longitude, 4326);
            var poly = point.BufferWithTolerance(radiusInMeters, 0.01, true); //0.01 is to simplify the polygon to keep only a few sides
            return poly;
        }

        public async Task<RadiusRegionsModel> GetAllRegionsInRadius(double latitude, double longitude, float radius, RegionSearchTypeCatalog searchType, RegionRadiusTypeCatalog radiusType)
        {
            RadiusRegionsModel model = new RadiusRegionsModel();
            var searchRegionPolygon = SqlGeography.Point(latitude, longitude, 4326);
            if (searchType == RegionSearchTypeCatalog.Intersects)
            {
                searchRegionPolygon = GetRegionRadius(latitude, longitude, radius, radiusType);
            }

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(DapperHelper.ConnectionStringValue()))
            {

                var queryParameters = new DynamicParameters();
                queryParameters.Add("@searchRegionPolygon", searchRegionPolygon.ToString());
                string countrySql = GetCountryInsideRadius();
                string stateSql = GetRegionsInsideRadiusQuery("State", searchType);
                string districtSql = GetRegionsInsideRadiusQuery("District", searchType);
                string tehsilSql = GetRegionsInsideRadiusQuery("Tehsil", searchType);
                string unionCouncilSql = GetRegionsInsideRadiusQuery("UnionCouncil", searchType);

                model.Countries = (await connection.QueryAsync<BaseBriefModel>(countrySql, queryParameters)).ToList();
                model.States = (await connection.QueryAsync<BaseBriefModel>(stateSql, queryParameters)).ToList();
                model.Districts = (await connection.QueryAsync<BaseBriefModel>(districtSql, queryParameters)).ToList();
                model.Tehsils = (await connection.QueryAsync<BaseBriefModel>(tehsilSql, queryParameters)).ToList();
                model.UnionCouncils = (await connection.QueryAsync<BaseBriefModel>(unionCouncilSql, queryParameters)).ToList();
            }
            return model;

        }
        private string GetRegionsInsideRadiusQuery(string tableName, RegionSearchTypeCatalog searchType)
        {
            return $"Select * from {tableName} where Geometry.MakeValid().ST{searchType}( geometry::STGeomFromText(@searchRegionPolygon, 4326))=1";
        }
        private string GetCountryInsideRadius()// There is only one country right niw
        {
            return $"Select * from Country";
        }
    }
}
