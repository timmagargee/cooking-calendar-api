using CookingCalendarApi.DomainModels;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using CookingCalendarApi.Models.DomainModels;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CookingCalendarApi.Repositories
{
    public interface IRecipeRepository
    {
        Task<int> AddRecipe(NewRecipe recipe);
        Task<Recipe> GetRecipe(int recipeId);
        Task<IEnumerable<RecipeSummary>> GetRecipes();

        Task<int> CreateTag(NewTagDto tag);
        Task<IEnumerable<IdNameDto>> GetTags();
        Task UpdateRecipe(Recipe recipe);
        Task DeleteRecipe(int id);
        Task<IEnumerable<RecipeForAssignment>> GetRecipesForAssignment(DateTime startDate);
    }

    public class RecipeRepository : IRecipeRepository
    {
        private readonly AppConfig _sqlConfig;
        public RecipeRepository(AppConfig sqlConfig)
        {
            _sqlConfig = sqlConfig;
        }

        public async Task<int> AddRecipe(NewRecipe recipe)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QuerySingleAsync<int>(@"
                INSERT INTO Recipes ([Name], [AreMeasurementsStandard])
                OUTPUT inserted.[Id]
                VALUES (@Name, 1);"
                , new { recipe.Name }
            );
        }

        public async Task<Recipe> GetRecipe(int recipeId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"
                SELECT [Id], [Name], [Description], [Servings], [AreMeasurementsStandard]
                FROM [dbo].[Recipes]
                WHERE [Id] = @RecipeId;

                SELECT rt.[Id], rt.[TagId], rt.[SortOrder], t.[Name]
                FROM [dbo].[RecipeTags] rt
                INNER JOIN [dbo].[Tags] t ON rt.[TagId] = t.[Id]
                WHERE rt.RecipeId = @RecipeId;

                SELECT ri.[Id]
	                , ri.[IngredientId]
	                , ri.[SortOrder]
	                , ri.[Amount]
	                , ri.[AmountNumerator]
	                , ri.[AmountDenominator]
                    , ri.[Description]
	                , i.[Name] AS [Ingredient]
	                , m.[Id] AS [Measurement]
                FROM [dbo].[RecipeIngredients] ri
                INNER JOIN [dbo].[Ingredients] i ON ri.[IngredientId] = i.[Id]
                INNER JOIN [dbo].[Measurements] m ON ri.[MeasurementId] = m.[Id]
                WHERE ri.[RecipeId] = @RecipeId;

                SELECT [Id], [Step], [SortOrder]
                FROM [dbo].[Steps] 
                WHERE [RecipeId] = @RecipeId;"
                , new { RecipeId = recipeId }
            );

            var recipe = multi.ReadFirst<Recipe>();
            recipe.Tags = multi.Read<RecipeTag>();
            recipe.Ingredients = multi.Read<RecipeIngredient>();
            recipe.Steps = multi.Read<RecipeStep>();
            return recipe;
        }

        public async Task<IEnumerable<RecipeSummary>> GetRecipes()
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"
                SELECT [Id]
                      ,[Name]
                INTO #TT
                  FROM [dbo].[Recipes];

                SELECT * FROM #TT;

                SELECT rt.[RecipeId], t.[Name], rt.[SortOrder]
                FROM [dbo].[RecipeTags] rt
                INNER JOIN [dbo].[Tags] t ON rt.[TagId] = t.[Id]
                WHERE rt.RecipeId IN (SELECT [Id] FROM #TT);

                SELECT ri.[RecipeId], i.[Name], ri.[SortOrder], i.[isMeat], i.[isDairy], i.[isGluten]
                FROM [dbo].[RecipeIngredients] ri
                INNER JOIN [dbo].[Ingredients] i ON ri.[IngredientId] = i.[Id]
                WHERE ri.[RecipeId] IN (SELECT [Id] FROM #TT);"
            );

            var baseRecipes = multi.Read<IdNameDto>();
            var tags = multi.Read<RecipeBasicItem>();
            var ingredients = multi.Read<RecipeIngredientWithDietaryInfo>();

            var recipes = new List<RecipeSummary>();
            foreach (var recipe in baseRecipes)
            {
                var ings = ingredients.Where(x => x.RecipeId == recipe.Id);
                recipes.Add(new RecipeSummary(recipe.Id, recipe.Name, tags.GetNameList(recipe.Id), ings.GetNameList(recipe.Id)
                    , ings.All(x => !x.isMeat)
                    , ings.All(x => !x.isDairy)
                    , ings.All(x => !x.isGluten)
                    )
                );
            }

            return recipes;
        }

        public async Task<IEnumerable<RecipeForAssignment>> GetRecipesForAssignment(DateTime startDate)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"
                SELECT r.[Id], MAX(cm.[MealDate]) AS [LastMade]
                INTO #TT
                FROM [dbo].[Recipes] r
                LEFT JOIN [dbo].[CalendarMeals] cm ON cm.[recipeId] = r.[Id]     
                WHERE (cm.[Id] IS NULL OR cm.[IsUserAssigned] = 1 OR cm.[MealDate] < @StartDate)
                GROUP BY r.[Id];

                SELECT * FROM #TT;

                SELECT rt.[RecipeId], t.[Id]
                FROM [dbo].[RecipeTags] rt
                INNER JOIN [dbo].[Tags] t ON rt.[TagId] = t.[Id]
                WHERE rt.RecipeId IN (SELECT [Id] FROM #TT);

                SELECT ri.[RecipeId], ri.[IngredientId] 
                FROM [dbo].[RecipeIngredients] ri
                WHERE ri.[RecipeId] IN (SELECT [Id] FROM #TT);"
                , new { startDate }
            );

            var recipes = multi.Read<RecipeForAssignment>().ToList();
            var tags = multi.Read<RecipeBasicItemId>();
            var ingredients = multi.Read<RecipeBasicItemId>();

            foreach(var recipe in recipes)
            {
                recipe.TagIds = tags.Where(x => x.RecipeId == recipe.Id).Select(x => x.Id);
                recipe.IngredientIds = ingredients.Where(x => x.RecipeId == recipe.Id).Select(x => x.Id);
            }

            return recipes;
        }

        public async Task UpdateRecipe(Recipe recipe)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);
            await conn.OpenAsync();
            await using var trans = await conn.BeginTransactionAsync();

            await conn.ExecuteAsync(@"
                UPDATE [dbo].[Recipes] SET
	                [Name] = @Name
	                , [Description] = @Description
	                , [Servings] = @Servings
	                , [AreMeasurementsStandard] = @AreMeasurementsStandard
                WHERE [Id] = @RecipeId;

                DELETE FROM [dbo].[RecipeTags] WHERE [RecipeId] = @RecipeId;
                DELETE FROM [dbo].[RecipeIngredients] WHERE [RecipeId] = @RecipeId;
                DELETE FROM [dbo].[Steps] WHERE [RecipeId] = @RecipeId;"
                , new { RecipeId = recipe.Id, recipe.Name, recipe.Description, recipe.Servings, recipe.AreMeasurementsStandard }
                , trans
            );

            await conn.ExecuteAsync(@$"
                INSERT INTO [dbo].[RecipeTags] ([RecipeId], [TagId], [SortOrder])
                VALUES ({recipe.Id}, @TagId, @SortOrder);"
                , recipe.Tags
                , trans
            );

            await conn.ExecuteAsync(@$"
                INSERT INTO [dbo].[RecipeIngredients] (
	                [RecipeId]
	                , [IngredientId]
	                , [MeasurementId]
	                , [SortOrder]
	                , [Amount]
	                , [AmountNumerator]
	                , [AmountDenominator]
	                , [Description]
                ) VALUES (
	                {recipe.Id}
	                , @IngredientId
	                , @Measurement
	                , @SortOrder
	                , @Amount
	                , @AmountNumerator
	                , @AmountDenominator
	                , @Description
                );"
                , recipe.Ingredients
                , trans
            );

            await conn.ExecuteAsync(@"
                INSERT INTO [dbo].[Steps] ([RecipeId], [Step], [SortOrder])
                VALUES (@RecipeId, @Step, @SortOrder);"
                    , recipe.Steps.Select(x => new { x.Step, x.SortOrder, RecipeId = recipe.Id})
                    , trans
                );

            await trans.CommitAsync();
        }

        public async Task DeleteRecipe(int id)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@"
                DELETE FROM Recipes WHERE id = @id"
                , new { id }
            );
        }

        #region Tags
        public async Task<int> CreateTag(NewTagDto tag)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QueryFirstAsync<int>(@"
                INSERT INTO Tags ([Name])
                OUTPUT inserted.[Id]
                VALUES (@Name )"
                , new { tag.Name }
            );
        }

        public async Task<IEnumerable<IdNameDto>> GetTags()
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QueryAsync<IdNameDto>(@"
                SELECT [Id]
                      ,[Name]
                FROM [dbo].[Tags]"
            );
        }

        #endregion
    }
}
