using FluentMigrator;

namespace HubDatabaseMigrations.migrations
{
    [Migration(2)]
    public class M002_ShoppingRename : Migration
    {
        public override void Up()
        {
            Rename.Column("Item").OnTable("ShoppingListEnteredItem").To("Name");
        }

        public override void Down()
        {
            Rename.Column("Name").OnTable("ShoppingListEnteredItem").To("Item");
        }
    }
}
