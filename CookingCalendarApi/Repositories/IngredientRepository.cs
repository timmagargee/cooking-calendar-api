using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CookingCalendarApi.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<IngredientBase>> GetIngredients();
        Task<int> AddIngredient(Ingredient ing);
    }
    public class IngredientRepository : IIngredientRepository
    {
        private readonly AppConfig _sqlConfig;
        public IngredientRepository(AppConfig sqlConfig)
        {
            _sqlConfig = sqlConfig;
        }

        public async Task<IEnumerable<IngredientBase>> GetIngredients()
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QueryAsync<IngredientBase>(@"
                SELECT i.[Id]
	                , i.[Name]
                FROM [dbo].[Ingredients] i"
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
                ) OUTPUT inserted.id
                VALUES (
                    @Name
	                , @isMeat
	                , @isDairy
	                , @isGluten
                ); "
                , new { ing.Name, ing.IsMeat, ing.IsDairy, ing.IsGluten }
            );
        }
    }
}
