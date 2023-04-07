using CookingCalendarApi.Interfaces;
using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CookingCalendarApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SqlServerConfig _sqlConfig;
        public AuthRepository(SqlServerConfig sqlConfig) {
            _sqlConfig = sqlConfig;
        }

        public async Task<int?> Login(User user)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var userId = await conn.QuerySingleOrDefaultAsync<int>("GetUserId", new
            {
                user.Username, user.Password,
            }, commandType: CommandType.StoredProcedure);

            return userId;
        }

        public async Task<int?> CreateUser(User user)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var userId = await conn.QuerySingleOrDefaultAsync<int>("AddUser", new
            {
                user.Username, user.Password, user.Email, user.FirstName, user.LastName
            }, commandType: CommandType.StoredProcedure);

            return userId;
        }
    }
}
