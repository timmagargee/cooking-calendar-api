using FluentMigrator;

namespace HubDatabaseMigrations.migrations
{
    [Migration(4)]
    public class M004_AddCascadeDeletes : Migration
    {
        public override void Up()
        {

            Delete.ForeignKey("FK__ShoppingL__UserI__0D7A0286").OnTable("ShoppingList");
            Create.ForeignKey("FK_ShoppingList_Users").FromTable("ShoppingList").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
            Delete.ForeignKey("FK__ShoppingL__Shopp__114A936A").OnTable("ShoppingListGeneratedItem");
            Create.ForeignKey("FK_ShoppingListGeneratedItem_ShoppingList").FromTable("ShoppingListGeneratedItem").ForeignColumn("ShoppingListId").ToTable("ShoppingList").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
            Delete.ForeignKey("FK__ShoppingL__Shopp__17036CC0").OnTable("ShoppingListEnteredItem");
            Create.ForeignKey("FK_ShoppingListEnteredItem_ShoppingList").FromTable("ShoppingListEnteredItem").ForeignColumn("ShoppingListId").ToTable("ShoppingList").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);

            Delete.ForeignKey("FK__Recipes__UserId__6B24EA82").OnTable("Recipes");
            Create.ForeignKey("FK_Recipes_Users").FromTable("Recipes").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);

            Delete.ForeignKey("FK__Calendars__UserI__7F2BE32F").OnTable("Calendars");
            Create.ForeignKey("FK_Calendars_Users").FromTable("Calendars").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
            Delete.ForeignKey("FK__CalendarC__Calen__03F0984C").OnTable("CalendarCategories");
            Create.ForeignKey("FK_CalendarCategories_Calendars").FromTable("CalendarCategories").ForeignColumn("CalendarId").ToTable("Calendars").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);

            Delete.ForeignKey("FK__CalendarM__Recip__09A971A2").OnTable("CalendarMeals");
            Create.ForeignKey("FK_CalendarMeals_Recipes").FromTable("CalendarMeals").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);

            Delete.ForeignKey("FK__Tags__UserId__72C60C4A").OnTable("Tags");
            Create.ForeignKey("FK_Tags_Users").FromTable("Tags").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Tags_Users").OnTable("Tags");
            Create.ForeignKey("FK__Tags__UserId__72C60C4A").FromTable("Tags").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id");

            Delete.ForeignKey("FK_CalendarMeals_Recipes").OnTable("CalendarMeals");
            Create.ForeignKey("FK__CalendarM__Recip__09A971A2").FromTable("CalendarMeals").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id");

            Delete.ForeignKey("FK_CalendarCategories_Calendars").OnTable("CalendarCategories");
            Create.ForeignKey("FK__CalendarC__Calen__03F0984C").FromTable("CalendarCategories").ForeignColumn("CalendarId").ToTable("Calendars").PrimaryColumn("Id");
            Delete.ForeignKey("FK_Calendars_Users").OnTable("Calendars");
            Create.ForeignKey("FK__Calendars__UserI__7F2BE32F").FromTable("Calendars").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id");

            Delete.ForeignKey("FK_Recipes_Users").OnTable("Recipes");
            Create.ForeignKey("FK__Recipes__UserId__6B24EA82").FromTable("Recipes").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id");

            Delete.ForeignKey("FK_ShoppingListEnteredItem_ShoppingList").OnTable("ShoppingListEnteredItem");
            Create.ForeignKey("FK__ShoppingL__Shopp__17036CC0").FromTable("ShoppingListEnteredItem").ForeignColumn("ShoppingListId").ToTable("ShoppingList").PrimaryColumn("Id");
            Delete.ForeignKey("FK_ShoppingListGeneratedItem_ShoppingList").OnTable("ShoppingListGeneratedItem");
            Create.ForeignKey("FK__ShoppingL__Shopp__114A936A").FromTable("ShoppingListGeneratedItem").ForeignColumn("ShoppingListId").ToTable("ShoppingList").PrimaryColumn("Id");
            Delete.ForeignKey("FK_ShoppingList_Users").OnTable("ShoppingList");
            Create.ForeignKey("FK__ShoppingL__UserI__0D7A0286").FromTable("ShoppingList").ForeignColumn("UserId").ToTable("Users").PrimaryColumn("Id");

        }
    }
}
