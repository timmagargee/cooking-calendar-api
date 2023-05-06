using FluentMigrator;

namespace HubDatabaseMigrations.migrations
{
    [Migration(3)]
    public class M003_AddSettings : Migration
    {
        public override void Up()
        {
            Alter.Table("Users")
                .AddColumn("DefaultShoppingDay").AsInt16().Nullable()
                .AddColumn("DefaultServings").AsInt16().Nullable();

            Rename.Column("ServingSize").OnTable("Recipes").To("Servings");
        }

        public override void Down()
        {
            Rename.Column("Servings").OnTable("Recipes").To("ServingSize");

            Delete.Column("DefaultShoppingDay")
                .Column("DefaultServings")
                .FromTable("Users");
        }
    }
}
