using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CookingCalendarApi.Repositories
{
    public interface ISettingsRepository
    {
        Task<UserSettingsDto> GetUserSettings(int id);
        Task UpdateUserSettings(UserSettingsDto settings);
        Task DeleteUserSettings(int id);
        Task<bool> DoesUsernameExist(int id, string username);
    }
    public class SettingsRepository: ISettingsRepository
    {
        private readonly SqlServerConfig _sqlConfig;
        public SettingsRepository(SqlServerConfig sqlConfig)
        {
            _sqlConfig = sqlConfig;
        }

        public async Task<UserSettingsDto> GetUserSettings(int id)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QuerySingleAsync<UserSettingsDto>(@"           
                SELECT [UserName]
	                , [Email]
	                , [FirstName]
	                , [LastName]
	                , [isDefaultMeasurementStandard]
                    , [DefaultShoppingDay]
                    , [DefaultServings]
                FROM [dbo].[Users]
                WHERE [Id] = @Id;"
                , new { id }
            );
        }
        public async Task UpdateUserSettings(UserSettingsDto settings)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@"           
                UPDATE [dbo].[Users] SET
	                [UserName] = @UserName
	                , [Email] = @Email
	                , [FirstName] = @FirstName
	                , [LastName] = @LastName
	                , [isDefaultMeasurementStandard] = @isDefaultMeasurementStandard
                    , [DefaultShoppingDay] = @DefaultShoppingDay
                    , [DefaultServings] = @DefaultServings
                WHERE [Id] = @Id;"
                , settings
            );
        }

        public async Task DeleteUserSettings(int id)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);
            await conn.OpenAsync();
            await using var trans = await conn.BeginTransactionAsync();

            await conn.ExecuteAsync(@"           
                DELETE FROM [dbo].[Users] WHERE [Id] = @id;"
                , new { id }, trans
            );
            await trans.CommitAsync();  
        }

        public async Task<bool> DoesUsernameExist(int id, string username)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return (await conn.QuerySingleAsync<int>(@"
            SELECT COUNT(*) 
            FROM [dbo].[Users]
            WHERE [Id] != @Id AND [Username] = @Username", 
            new{ id, username })) > 0;
        }
    }
}
