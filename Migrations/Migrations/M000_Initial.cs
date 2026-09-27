using FluentMigrator;
using System.Data;

namespace HubDatabaseMigrations.migrations
{
    [Migration(0)]
    public class M000_Initial : Migration
    {
        public override void Up()
        {
            Create.Table("Measurements")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("Name").AsString(64).NotNullable()
                .WithColumn("IsStandard").AsBoolean().NotNullable();

            Create.Table("Ingredients")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(64).NotNullable()
                .WithColumn("IsMeat").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("IsDairy").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("isGluten").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Table("Recipes")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Description").AsString(4000).Nullable()
                .WithColumn("Servings").AsInt16().NotNullable().WithDefaultValue(1)
                .WithColumn("AreMeasurementsStandard").AsBoolean().NotNullable().WithDefaultValue(1);

            Create.Table("RecipeIngredients")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("Recipes", "Id").OnDelete(Rule.Cascade)
                .WithColumn("IngredientId").AsInt64().NotNullable().ForeignKey("Ingredients", "Id").OnDelete(Rule.Cascade)
                .WithColumn("MeasurementId").AsInt64().NotNullable().ForeignKey("Measurements", "Id").OnDelete(Rule.Cascade)
                .WithColumn("SortOrder").AsInt16().NotNullable()
                .WithColumn("Amount").AsDouble().Nullable()
                .WithColumn("AmountNumerator").AsInt16().Nullable()
                .WithColumn("AmountDenominator").AsInt16().Nullable()
                .WithColumn("Description").AsString(32).Nullable();


            Create.Table("Tags")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(64).NotNullable();

            Create.Table("RecipeTags")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("Recipes", "Id").OnDelete(Rule.Cascade)
                .WithColumn("TagId").AsInt64().NotNullable().ForeignKey("Tags", "Id").OnDelete(Rule.Cascade)
                .WithColumn("SortOrder").AsInt16().NotNullable();

            Create.Table("Steps")
                  .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                  .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("Recipes", "Id").OnDelete(Rule.Cascade)
                  .WithColumn("Step").AsString(4000).NotNullable()
                  .WithColumn("SortOrder").AsInt16().NotNullable();

            Create.Table("Calendars")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("LastGenerated").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
              .WithColumn("isMonthDefaultView").AsBoolean().NotNullable().WithDefaultValue(0);

            Create.Table("CalendarCategories")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("CalendarId").AsInt64().NotNullable().ForeignKey("Calendars", "Id").OnDelete(Rule.Cascade)
              .WithColumn("DayOfWeek").AsInt16().NotNullable()
              .WithColumn("Name").AsString(64).NotNullable()
              .WithColumn("CategoryType").AsInt16().NotNullable()
              .WithColumn("TagId").AsInt64().Nullable().ForeignKey("Tags", "Id")
              .WithColumn("IngredientId").AsInt64().Nullable().ForeignKey("Ingredients", "Id");

            Create.Table("CalendarMeals")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("CalendarId").AsInt64().NotNullable().ForeignKey("Calendars", "Id").OnDelete(Rule.Cascade)
              .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("Recipes", "Id")
              .WithColumn("MealDate").AsDate().NotNullable()
              .WithColumn("IsUserAssigned").AsBoolean().NotNullable().WithDefaultValue(false);

            Create.Table("ShoppingList")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("StartDate").AsDate().NotNullable()
              .WithColumn("EndDate").AsDate().NotNullable()
              .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now);

            Create.Table("ShoppingListGeneratedItem")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("ShoppingListId").AsInt64().NotNullable().ForeignKey("ShoppingList", "Id").OnDelete(Rule.Cascade)
              .WithColumn("IngredientId").AsInt64().NotNullable().ForeignKey("Ingredients", "Id")
              .WithColumn("MeasurementId").AsInt64().NotNullable().ForeignKey("Measurements", "Id")
              .WithColumn("Amount").AsDouble().NotNullable()
              .WithColumn("IsChecked").AsBoolean().NotNullable().WithDefaultValue(0);

            Create.Table("ShoppingListEnteredItem")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("ShoppingListId").AsInt64().NotNullable().ForeignKey("ShoppingList", "Id").OnDelete(Rule.Cascade)
              .WithColumn("Category").AsInt16().NotNullable()
              .WithColumn("Name").AsString(64).NotNullable()
              .WithColumn("IsChecked").AsBoolean().NotNullable().WithDefaultValue(0);

            Execute.Sql(@"
                INSERT INTO [dbo].[Measurements] ([Id], [Name], [isStandard])
                VALUES 
	                ('0', 'Amount', 1),
	                ('1', 'Tsp', 1),
	                ('2', 'Tbl', 1),
	                ('3', 'FlOz', 1),
	                ('4', 'Gill', 1),
	                ('5', 'Cup', 1),
	                ('6', 'Pt', 1),
	                ('7', 'Qt', 1),
	                ('8', 'Gal', 1),
	                ('9', 'Lb', 1),
	                ('10', 'Oz', 1),
	                ('11', 'In', 1),
	                ('12', 'Yd', 1),
	                ('13', 'F', 1),
	                ('101', 'ML', 0),
	                ('102', 'Lb', 0),
	                ('103', 'DL', 0),
	                ('104', 'MM', 0),
	                ('105', 'CM', 0),
	                ('106', 'M', 0),
	                ('107', 'Celcius', 0);
            ");
        }

        public override void Down()
        {
            Delete.Table("ShoppingListEnteredItem");
            Delete.Table("ShoppingListGeneratedItem");
            Delete.Table("ShoppingList");
            Delete.Table("CalendarMeals");
            Delete.Table("CalendarCategories");
            Delete.Table("Calendars");
            Delete.Table("Steps");
            Delete.Table("RecipeTags");
            Delete.Table("Tags");
            Delete.Table("RecipeIngredients");
            Delete.Table("Recipes");
            Delete.Table("Ingredients");
            Delete.Table("Measurements");
        }
    }
}
