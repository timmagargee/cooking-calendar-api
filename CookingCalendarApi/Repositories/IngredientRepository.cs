using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CookingCalendarApi.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<IngredientBase>> GetIngredients(int userId);
        Task<int> AddIngredient(Ingredient ing);
        Task<int> AddUserIngredient(int userId, int ingedientId);
    }
    public class IngredientRepository : IIngredientRepository
    {
        private readonly SqlServerConfig _sqlConfig;
        public IngredientRepository(SqlServerConfig sqlConfig)
        {
            _sqlConfig = sqlConfig;
        }

        public async Task<IEnumerable<IngredientBase>> GetIngredients(int userId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QueryAsync<IngredientBase>(@"
                SELECT ui.[Id] AS [UserIngredientId]
                    , i.[Id]
	                , i.[Name]
                FROM [dbo].[Ingredients] i
                LEFT  JOIN [dbo].[UserIngredients] ui ON i.[Id] = ui.[IngredientId]
                WHERE i.[isUserMade] = 0 OR ui.[UserId] = @UserId
                ORDER BY ui.[id]"
                , new { userId }
            );
        }

        public async Task<int> AddIngredient(Ingredient ing)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QuerySingleAsync<int>(@"
                INSERT INTO Ingredients (
	                [Name]
	                , [isMeat]
	                , [isDairy]
	                , [isGluten]
	                , [isUserMade]
                ) OUTPUT inserted.id
                VALUES (
                    @Name
	                , @isMeat
	                , @isDairy
	                , @isGluten
	                , 1
                ); "
                , new { ing.Name, ing.IsMeat, ing.IsDairy, ing.IsGluten }
            );
        }

        public async Task<int> AddUserIngredient(int userId, int ingredientId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QuerySingleAsync<int>(@"
                INSERT INTO UserIngredients (
	                [UserId]
	                , [IngredientId]
                ) OUTPUT inserted.id
                VALUES (
                    @UserId
	                , @IngredientId
                ); "
                , new { userId, ingredientId }
            );
        }
    }
}
