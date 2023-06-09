﻿using CookingCalendarApi.DomainModels;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using CookingCalendarApi.StartupClasses;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CookingCalendarApi.Repositories
{
    public interface ICalendarRepository
    {
        Task<IEnumerable<MealDto>> GetCalendarMealDtos(int calendarId, DateFilter filters);
        Task<CalendarDto> GetCalendar(int userId);
        Task CreateNewCalendar(int userId);
        Task UpdateCategory(Category category);
        Task AssignRecipeToDate(AssignMealDto meal, int userId);
        Task<IEnumerable<Meal>> GetCalendarMeals(int calendarId, DateFilter filters);
        Task<IEnumerable<MealDto>> GetAllMealsInRange(int userId, DateFilter filters);
        Task AddMeals(IEnumerable<Meal> meals, int calendarId);
        Task UpdateMeals(IEnumerable<Meal> meals);
    }
    public class CalendarRepository : ICalendarRepository
    {
        private readonly SqlServerConfig _sqlConfig;
        public CalendarRepository(SqlServerConfig sqlConfig)
        {
            _sqlConfig = sqlConfig;
        }
        public async Task<CalendarDto?> GetCalendar(int userId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"           
                SELECT [Id], [LastGenerated] 
                INTO #TT
                FROM [dbo].[Calendars]
                WHERE UserId = @UserId;

                SELECT * FROM #TT;

                SELECT cc.* 
                FROM [dbo].[CalendarCategories] cc
                INNER JOIN #TT c ON c.[Id] = cc.[CalendarId];"
                , new { UserId = userId }
            );

            var calendar = multi.ReadFirstOrDefault<CalendarDto>();
            if (calendar != null)
            {
                calendar.Categories = multi.Read<Category>();
            }
            return calendar;
        }

        public async Task<IEnumerable<MealDto>> GetCalendarMealDtos(int calendarId, DateFilter filters)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"           
                SELECT * 
                INTO #TT
                FROM [dbo].[CalendarMeals]
                WHERE [CalendarId] = @CalendarId
                    AND MealDate >= @StartDate
                    AND MealDate <= @EndDate;
                
                SELECT * FROM #TT;

                SELECT r.[Id]
                   , r.[Name]
                FROM [dbo].[Recipes] r
                INNER JOIN #TT cm ON cm.[RecipeId] = r.[Id];
"
                , new { CalendarId = calendarId, filters.StartDate, filters.EndDate }
            );

            var meals = multi.Read<Meal>();
            var recipes = multi.Read<RecipeSummary>();

            return meals.Select(x => new MealDto() { Id = x.Id, IsUserAssigned = x.IsUserAssigned, MealDate = x.MealDate, Recipe = recipes.First(y => y.Id == x.RecipeId) });
        }

        public async Task<IEnumerable<MealDto>> GetAllMealsInRange(int userId, DateFilter filters)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            var multi = await conn.QueryMultipleAsync(@"           
                SELECT cm.* 
                INTO #TT
                FROM [dbo].[CalendarMeals] cm
                INNER JOIN [dbo].[Calendars] c ON c.[Id] = cm.[CalendarId]
                WHERE c.[UserId] = @userId 
                    AND cm.MealDate >= @StartDate
                    AND cm.MealDate <= @EndDate;
                
                SELECT * FROM #TT;

                SELECT r.[Id]
                   , r.[Name]
                INTO #RTT
                FROM [dbo].[Recipes] r
                INNER JOIN #TT cm ON cm.[RecipeId] = r.[Id];

                SELECT * FROM #RTT;
                
                SELECT rt.[RecipeId], t.[Name], rt.[SortOrder]
                FROM [dbo].[RecipeTags] rt
                INNER JOIN [dbo].[Tags] t ON rt.[TagId] = t.[Id]
                WHERE rt.RecipeId IN (SELECT [Id] FROM #RTT);

                SELECT ri.[RecipeId], i.[Name], ri.[SortOrder]
                FROM [dbo].[RecipeIngredients] ri
                INNER JOIN [dbo].[UserIngredients] ui ON ri.[IngredientId] = ui.[Id]
                INNER JOIN [dbo].[Ingredients] i ON ui.[IngredientId] = i.[Id]
                WHERE ri.[RecipeId] IN (SELECT [Id] FROM #RTT);"
                , new { userId, StartDate = filters.StartDate.Date, EndDate = filters.EndDate.Date }
            );

            var meals = multi.Read<Meal>();
            var recipes = multi.Read<RecipeSummary>().ToList();

            var recipesTags = multi.Read<RecipeBasicItem>();
            var recipesIngredients = multi.Read<RecipeBasicItem>();

            foreach (var recipe in recipes)
            {
                recipe.Tags = recipesTags.GetNameList(recipe.Id);
                recipe.Ingredients = recipesIngredients.GetNameList(recipe.Id);
            }

            return meals.Select(x => new MealDto() { Id = x.Id, IsUserAssigned = x.IsUserAssigned, MealDate = x.MealDate, Recipe = recipes.First(y => y.Id == x.RecipeId) });
        }

        public async Task<IEnumerable<Meal>> GetCalendarMeals(int calendarId, DateFilter filters)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            return await conn.QueryAsync<Meal>(@"           
                SELECT * 
                FROM [dbo].[CalendarMeals]
                WHERE [CalendarId] = @CalendarId
                    AND MealDate >= @StartDate
                    AND MealDate <= @EndDate;"
                , new { CalendarId = calendarId, filters.StartDate, filters.EndDate }
            );
        }

        public async Task CreateNewCalendar(int userId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);
            await conn.OpenAsync();
            await using var trans = await conn.BeginTransactionAsync();

            var calId = await conn.QuerySingleAsync<int>(@"           
                INSERT INTO [dbo].[Calendars] (
	                [UserId]
	                , [LastGenerated]
	                , [isMonthDefaultView]
                ) OUTPUT INSERTED.Id VALUES (
	                @UserId
	                , GETDATE()
	                , 1
                );"
                , new { userId }
                , trans
            );

            var valueString = string.Join(',',
                Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(x => $"({calId}, {(int)x}, 3, 'Unplanned')"));

            await conn.ExecuteAsync(@$"           
                INSERT INTO [dbo].[CalendarCategories] (
	                [CalendarId]
	                , [DayOfWeek]
	                , [CategoryType]
                    , [Name]
                ) OUTPUT INSERTED.Id VALUES 
                {valueString};"
                , transaction: trans
            );

            await trans.CommitAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@$"      
                UPDATE [dbo].[CalendarCategories] SET
                [Name] = @Name
                , [DayOfWeek] = @DayOfWeek
                , [CategoryType] = @CategoryType
                , [TagId] = @TagId
                , [IngredientId] = @IngredientId
                WHERE [Id] = @Id;"
                , category
            );
        }

        public async Task AssignRecipeToDate(AssignMealDto meal, int userId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            if (meal.MealId == null)
            {
                var calendarId = await conn.QuerySingleAsync<int>(@$"
                    SELECT [Id]
                    FROM [dbo].[Calendars]
                    WHERE UserId = @UserId;", new { userId });

                var testId = await conn.QuerySingleOrDefaultAsync<int?>(@$"
                    SELECT [Id]
                    FROM [dbo].[CalendarMeals]
                    WHERE [CalendarId] = @CalendarId
                        AND [MealDate] = @MealDate;", new { meal.MealDate, calendarId });

                if (testId != null)
                {
                    meal.MealId = testId;
                }
                else
                {
                    await conn.ExecuteAsync(@$"      
                        INSERT INTO [dbo].[CalendarMeals] ([CalendarId], [RecipeId], [MealDate], [IsUserAssigned])
                        VALUES (@CalendarId, @RecipeId, @MealDate, 1);"
                        , new { meal.RecipeId, meal.MealDate, calendarId }
                    );
                }
            }

            if (meal.MealId != null)
            {
                await conn.ExecuteAsync(@$"      
                UPDATE [dbo].[CalendarMeals] SET
                    [RecipeId] = @RecipeId
                    , [IsUserAssigned] = 1
                WHERE [Id] = @MealId;"
                    , meal
                );
            }
        }

        public async Task AddMeals(IEnumerable<Meal> meals, int calendarId)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@$"      
                INSERT INTO [dbo].[CalendarMeals] ([CalendarId], [RecipeId], [MealDate], [IsUserAssigned])
                VALUES (@CalendarId, @RecipeId, @MealDate, 0);"
                , meals.Select(x => new { x.RecipeId, calendarId, x.MealDate })
            );
        }

        public async Task UpdateMeals(IEnumerable<Meal> meals)
        {
            using var conn = new SqlConnection(_sqlConfig.ConnectionString);

            await conn.ExecuteAsync(@$"      
            UPDATE [dbo].[CalendarMeals] SET
                [RecipeId] = @RecipeId
            WHERE [Id] = @MealId;"
                , meals.Select(x => new { MealId = x.Id, x.RecipeId })
            );
        }


    }
}
