using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EntityProvider
{
    public partial class DataAccess
    {
        public async Task<bool> UpdateRefreshToken(RefreshTokenModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(DapperConnectionString()))
            {
                try
                {
                    string sql = "SELECT * FROM dbo.RefreshToken WHERE UserId = @UserId;";
                    var queryParameters = new DynamicParameters();
                    queryParameters.Add("@UserId", model.UserId);
                    var existingToken = await connection.QueryFirstOrDefaultAsync<RefreshTokenModel>(sql, queryParameters);
                    if (existingToken != null)
                    {
                        await RemoveRefreshToken(existingToken);
                    }
                    return await AddRefreshToken(model, connection);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<bool> RemoveRefreshToken(RefreshTokenModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(DapperConnectionString()))
            {
                string sql = "Delete FROM dbo.RefreshToken WHERE Id = @Id;";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", model.Id);
                return await connection.ExecuteAsync(sql, queryParameters) > 0;
            }
        }
        public async Task<RefreshTokenModel> FindRefreshToken(string hashedToken)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(DapperConnectionString()))
            {
                string sql = "SELECT * FROM dbo.RefreshToken WHERE Id = @Id;";
                var queryParameters = new DynamicParameters();
                queryParameters.Add("@Id", hashedToken);
                return await connection.QueryFirstOrDefaultAsync<RefreshTokenModel>(sql, queryParameters);
            }
        }
        public async Task<List<RefreshTokenModel>> GetAllRefreshTokens()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(DapperConnectionString()))
            {
                string sql = "SELECT * FROM dbo.RefreshToken";
                return (await connection.QueryAsync<RefreshTokenModel>(sql)).ToList();
            }
        }
        private async Task<bool> AddRefreshToken(RefreshTokenModel token, IDbConnection connection)
        {
            string insertQuery = @"INSERT INTO dbo.RefreshToken ([ID], [UserId], [IssuedTime], [ExpiredTime], [ProtectedTicket]) VALUES (@Id, @UserId, @IssuedTime, @ExpiredTime, @ProtectedTicket)";
            var queryParameters = new DynamicParameters();
            queryParameters.Add("@Id", token.Id);
            queryParameters.Add("@UserId", token.UserId);
            queryParameters.Add("@IssuedTime", token.IssuedTime);
            queryParameters.Add("@ExpiredTime", token.ExpiredTime);
            queryParameters.Add("@ProtectedTicket", token.ProtectedTicket);
            return await connection.ExecuteAsync(insertQuery, queryParameters) > 0;

        }
    }
}
