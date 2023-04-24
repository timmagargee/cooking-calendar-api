using FluentMigrator;

namespace HubDatabaseMigrations.migrations
{
    [Migration(0)]
    public class M000_Initial : Migration
    {
        public override void Up()
        {
			Execute.Sql(@"
				CREATE TABLE Users (
					Id int IDENTITY(1,1) PRIMARY KEY,
					UserName nvarchar(32) NOT NULL,
					PasswordHash nvarchar(4000) NOT NULL,
					Salt uniqueIdentifier NOT NULL,
					Email nvarchar(255) NOT NULL,
					FirstName nvarchar(32) NOT NULL,
					LastName nvarchar(32) NOT NULL,
					isDefaultMeasurementStandard bit DEFAULT 1 NOT NULL,
					IsDarkMode bit DEFAULT 0 NOT NULL
				);

				CREATE TABLE Measurements (
					Id int PRIMARY KEY,
					[Name] nvarchar(64) NOT NULL,
					isStandard bit NOT NULL
				);

				CREATE TABLE Ingredients (
					Id int IDENTITY(1,1) PRIMARY KEY,
					isUserMade bit,
					[Name] nvarchar(64) NOT NULL,
					isMeat bit DEFAULT 0 NOT NULL,
					isDairy bit DEFAULT 0 NOT NULL,
					isGluten bit DEFAULT 0 NOT NULL
				);

				CREATE TABLE UserIngredients (
					Id int IDENTITY(1,1) PRIMARY KEY,
					UserId int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
					IngredientId int FOREIGN KEY REFERENCES Ingredients(Id) NOT NULL
				);

				CREATE TABLE Recipes (
					Id int IDENTITY(1,1) PRIMARY KEY,
					UserId int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
					[Name] nvarchar(255) NOT NULL,
					[Description] nvarchar(4000),
					ServingSize tinyint NOT NULL
				);

				CREATE TABLE RecipeIngredients (
					Id int IDENTITY(1,1) PRIMARY KEY,
					RecipeId int FOREIGN KEY REFERENCES Recipes(Id) NOT NULL,
					IngredientId int FOREIGN KEY REFERENCES UserIngredients(Id) NOT NULL,
					MeasurementId int FOREIGN KEY REFERENCES Measurements(Id) NOT NULL,
					SortOrder tinyint NOT NULL,
					Amount float,
					AmountNumerator int,
					AmountDenominator int
				);

				CREATE TABLE Tags (
					Id int IDENTITY(1,1) PRIMARY KEY,
					UserId int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
					[Name] nvarchar(255) NOT NULL,
				);


				CREATE TABLE RecipeTags (
					Id int IDENTITY(1,1) PRIMARY KEY,
					RecipeId int FOREIGN KEY REFERENCES Recipes(Id) NOT NULL,
					TagId int FOREIGN KEY REFERENCES Tags(Id) NOT NULL,
					SortOrder tinyint 
				);

				CREATE TABLE Steps (
					Id int IDENTITY(1,1) PRIMARY KEY,
					RecipeId int FOREIGN KEY REFERENCES Recipes(Id) NOT NULL,
					Step nvarchar(4000) NOT NULL,
					SortOrder tinyint NOT NULL
				);

				--CREATE TABLE StepRecipeIngredients (
				--    Id int IDENTITY(1,1) PRIMARY KEY,
				--	RecipeId int FOREIGN KEY REFERENCES Recipes(Id) NOT NULL,
				--	StepNumber int,
				--	IngredientNumber int,
				--	AmountNumerator float,
				--	AmountDenominator int
				--);


				CREATE TABLE Calendars (
					Id int IDENTITY(1,1) PRIMARY KEY,
					UserId int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
					LastGenerated dateTime DEFAULT GETDATE() NOT NULL,
					isMonthDefaultView bit DEFAULT 0 NOT NULL,
				);

				CREATE TABLE CalendarCategories (
					Id int IDENTITY(1,1) PRIMARY KEY,
					CalendarId int FOREIGN KEY REFERENCES Calendars(Id) NOT NULL,
					[DayOfWeek] tinyint NOT NULL,
					[Name] nvarchar(255),
					CategoryType tinyint NOT NULL,
					TagId int FOREIGN KEY REFERENCES Tags(Id) NULL,
					IngredientId int FOREIGN KEY REFERENCES UserIngredients(Id) NULL
				);

				CREATE TABLE CalendarMeals (
					Id int IDENTITY(1,1) PRIMARY KEY,
					CalendarId int FOREIGN KEY REFERENCES Calendars(Id) NOT NULL,
					RecipeId int FOREIGN KEY REFERENCES Recipes(Id) NOT NULL,
					MealDate date NOT NULL,
					IsUserAssigned bit DEFAULT 0 NOT NULL
				);

				CREATE TABLE ShoppingList (
					Id int IDENTITY(1,1) PRIMARY KEY,
					UserId int FOREIGN KEY REFERENCES Users(Id) NOT NULL,
					StartDate date NOT NULL,
					EndDate date NOT NULL,
					CreatedOn dateTime DEFAULT GETDATE() NOT NULL
				);

				CREATE TABLE ShoppingListGeneratedItem (
					Id int IDENTITY(1,1) PRIMARY KEY,
					ShoppingListId int FOREIGN KEY REFERENCES ShoppingList(Id) NOT NULL,
					IngredientId int FOREIGN KEY REFERENCES UserIngredients(Id) NOT NULL,
					MeasurementId int FOREIGN KEY REFERENCES Measurements(Id) NOT NULL,
					Amount float NOT NULL,
					IsChecked bit DEFAULT 0 NOT NULL
				);

				CREATE TABLE ShoppingListEnteredItem (
					Id int IDENTITY(1,1) PRIMARY KEY,
					ShoppingListId int FOREIGN KEY REFERENCES ShoppingList(Id) NOT NULL,
					Category int NOT NULL,
					Item nvarchar(64),
					IsChecked bit DEFAULT 0 NOT NULL
				);

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
            "
            );
		}

        public override void Down()
        {
        }
    }
}
