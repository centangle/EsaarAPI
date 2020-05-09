using Catalogs;
using Models;
using Models.BriefModel;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Dapper;
using Microsoft.SqlServer.Types;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityProvider
{
    public partial class DataAccess
    {
        private async Task<SqlGeography> GetRegionRadius(double latitude, double longitude, float radius, RegionRadiusTypeCatalog radiusType)
        {
            //Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            var radiusInMeters = radius;
            if (radiusType == RegionRadiusTypeCatalog.Kilometers)
            {
                radiusInMeters = radius * 1000;
            }
            var point = SqlGeography.Point(latitude, longitude, 4326);
            return SqlGeography.Parse(await GetRadius(point, radiusInMeters));
            //poly = point.BufferWithTolerance(radiusInMeters, 0.01, true); 
        }

        public async Task<FilteredRegionsModel> FilterRegions(double latitude, double longitude, float radius, RegionSearchTypeCatalog searchType, RegionRadiusTypeCatalog? radiusType = null)
        {
            FilteredRegionsModel model = new FilteredRegionsModel();
            var searchRegionPolygon = SqlGeography.Point(latitude, longitude, 4326);
            if (searchType == RegionSearchTypeCatalog.Intersects)
            {
                searchRegionPolygon = await GetRegionRadius(latitude, longitude, radius, radiusType ?? RegionRadiusTypeCatalog.Meters);
            }

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(DapperConnectionString()))
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
        private string GetCountryInsideRadius()// There is only one country right now
        {
            return $"Select * from Country";
        }
        private async Task<string> GetRadius(SqlGeography point, float radius)
        {

            string connectionString = DapperConnectionString();
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                return (await connection.QueryAsync<string>("dbo.sp_GetCircularPolygon", new { @point = point, @radius = radius },
                    commandType: CommandType.StoredProcedure)).ToList().FirstOrDefault();
            }
        }
    }
}
