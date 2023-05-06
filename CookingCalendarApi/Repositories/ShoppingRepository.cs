using CookingCalendarApi.DomainModels;
using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace CookingCalendarApi.Repositories
{
    public interface IShoppingRepository
    {
        Task<ShoppingListDto> GetUserShoppingList(int userId);
        Task<IEnumerable<ShoppingIngredient>> GetShoppingIngredients(int userId, DateFilter dateFilters);
        Task AddGeneratedItems(int shoppinglistId, IEnumerable<GeneratedItem> items);
        Task<int> AddOrUpdateShoppingList(int userId, DateFilter dates);
        Task UpdateShoppingList(ShoppingListDto list);
        Task DeleteGeneratedItems(int shoppinglistId);
        Task ClearChecked(int shoppinglistId);
    }
    public class ShoppingRepository : IShoppingRepository
    {
        private readonly SqlServerConfig _sqlConfig;
        public ShoppingRepository(SqlServerConfig sqlConfig)
        {
            _sqlConfig = sqlConfig;
        }

        public async Task<ShoppingListDto> GetUserShoppingList(int userId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"
                SELECT [Id]
                    , [StartDate]
                    , [EndDate]
                    , [CreatedOn]
                INTO #TT
                  FROM [dbo].[ShoppingList]
                  WHERE [UserId] = @UserId;

                SELECT * FROM #TT;

                SELECT gi.[Id]
                    , gi.[IngredientId]
                    , gi.[MeasurementId] AS [MeasurementType]
                    , gi.[Amount]
                    , gi.[IsChecked]
                    , i.[Name]
                FROM [dbo].[ShoppingListGeneratedItem] gi
                INNER JOIN [dbo].[UserIngredients] ui ON gi.[IngredientId] = ui.[Id]
                INNER JOIN [dbo].[Ingredients] i ON ui.[IngredientId] = i.[Id]
                WHERE gi.[ShoppingListId] IN (SELECT [Id] FROM #TT);

                SELECT ei.[Id]
                    , ei.[Category]
                    , ei.[Name]
                    , ei.[IsChecked]
                FROM [dbo].[ShoppingListEnteredItem] ei
                WHERE ei.[ShoppingListId] IN (SELECT [Id] FROM #TT);"
                , new { userId }
            );

            var shoppingList = multi.ReadSingleOrDefault<ShoppingListDto>();
            if (shoppingList != null)
            {
                shoppingList.GeneratedItems = multi.Read<GeneratedItem>();
                shoppingList.EnteredItems = multi.Read<EnteredItem>();
            }
            return shoppingList;
        }

        public async Task<IEnumerable<ShoppingIngredient>> GetShoppingIngredients(int userId, DateFilter dateFilters)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QueryAsync<int, RecipeAmount, ShoppingIngredient>(@"
                SELECT 
                      ri.[IngredientId]
                      , ri.[MeasurementId] AS [Measurement]
                      , ri.[Amount] 
                      , ri.[AmountNumerator] AS [Numerator]
                      , ri.[AmountDenominator] AS [Denominator]
                      , r.[Servings]
                FROM [dbo].[RecipeIngredients] ri
                INNER JOIN [dbo].[Recipes] r ON r.[Id] = ri.[RecipeId]
                INNER JOIN [dbo].[CalendarMeals] cm ON r.[Id] = cm.[RecipeId]
                WHERE r.[UserId] = @UserId
	                AND cm.[MealDate] >= @StartDate 
	                AND cm.[MealDate] <= @EndDate
                ", (ingId, amount) =>
                {
                    return new ShoppingIngredient() { IngredientId = ingId, Amount = amount };
                }
                , new { userId, StartDate = dateFilters.StartDate.Date, EndDate = dateFilters.EndDate.Date }
                , splitOn: "Measurement"
            );
        }

        public async Task<int> AddOrUpdateShoppingList(int userId, DateFilter dates)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var id = await conn.QuerySingleOrDefaultAsync<int?>(@"
            SELECT [Id]
            FROM [dbo].[ShoppingList]
            WHERE [UserId] = @UserId;
            ", new { userId });

            if (id != null)
            {
                await conn.ExecuteAsync(@"
                UPDATE [dbo].[ShoppingList] SET
                    [StartDate] = @StartDate
                    , [EndDate] = @EndDate
                WHERE [Id] = @Id;", new { id, dates.StartDate, dates.EndDate }
                );

                return id.Value;
            }

            return await conn.QuerySingleAsync<int>(@"
            INSERT INTO [dbo].[ShoppingList] (
                [UserId]
                , [StartDate]
                , [EndDate]
            ) OUTPUT INSERTED.Id VALUES (
                @UserId
                , @StartDate
                , @EndDate 
            );", new { userId, dates.StartDate, dates.EndDate }
            );
        }

        public async Task ClearChecked(int shoppinglistId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@"
                DELETE FROM [dbo].[ShoppingListGeneratedItem] WHERE ShoppinglistId = @ShoppinglistId AND IsChecked = 1
                DELETE FROM [dbo].[ShoppingListEnteredItem] WHERE ShoppinglistId = @ShoppinglistId AND IsChecked = 1", new { shoppinglistId }
            );
        }


        public async Task DeleteGeneratedItems(int shoppinglistId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@"
                DELETE FROM [dbo].[ShoppingListGeneratedItem] WHERE ShoppinglistId = @ShoppinglistId", new { shoppinglistId }
            );
        }

        public async Task AddGeneratedItems(int shoppinglistId, IEnumerable<GeneratedItem> items)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@"
                INSERT INTO [dbo].[ShoppingListGeneratedItem] (
                  [ShoppingListId]
                  , [IngredientId]
                  , [MeasurementId]
                  , [Amount]
                ) VALUES (
                  @ShoppingListId
                  , @IngredientId
                  , @MeasurementId
                  , @Amount    
                );", items.Select((x) => new
            {
                shoppinglistId,
                x.IngredientId,
                x.Amount,
                MeasurementId = x.MeasurementType
            })
            );
        }

        public async Task UpdateShoppingList(ShoppingListDto list)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);
            await conn.OpenAsync();
            await using var trans = await conn.BeginTransactionAsync();

            var sql = string.Join('\n', list.GeneratedItems.Select(x => @$"
                UPDATE [dbo].[ShoppingListGeneratedItem] SET [IsChecked] = {(x.IsChecked ? 1 : 0)} WHERE [Id] = {x.Id}"));

            await conn.ExecuteAsync(sql, transaction: trans);

            var itemsToAdd = list.EnteredItems.Where(x => !x.Id.HasValue);
            if (itemsToAdd.Any())
            {
                sql = @"
                INSERT INTO [dbo].[ShoppingListEnteredItem] (
                  [ShoppingListId]
                  , [Category]
                  , [Name]
                  , [IsChecked]
                ) VALUES (
                  @ShoppingListId
                  , @Category
                  , @Name
                  , @IsChecked    
                );";
                await conn.ExecuteAsync(sql, itemsToAdd.Select(x => new
                {
                    ShoppingListId = list.Id,
                    x.Category,
                    x.Name,
                    x.IsChecked
                }), trans);
            }

            var itemsToUpdate = list.EnteredItems.Where(x => x.Id.HasValue);
            if (itemsToUpdate.Any())
            {
                sql = @"
                UPDATE [dbo].[ShoppingListEnteredItem] SET
                  [Name] = @Name
                  , [IsChecked] = @IsChecked
                WHERE [Id] = @Id;";

                await conn.ExecuteAsync(sql, itemsToUpdate.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsChecked
                }), trans);
            }

            await trans.CommitAsync();
        }
    }
}
