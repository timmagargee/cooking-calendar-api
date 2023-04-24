using FluentMigrator;

namespace HubDatabaseMigrations.migrations
{
    [Migration(1)]
    public class M001_AddDefaults : Migration
    {
        public override void Up()
        {
            Alter.Column("ServingSize").OnTable("Recipes").AsInt16().NotNullable().WithDefaultValue(1);

            Alter.Table("RecipeIngredients")
                .AddColumn("Description").AsString(32).Nullable();

            Alter.Table("Recipes").AddColumn("AreMeasurementsStandard").AsBoolean().NotNullable().WithDefaultValue(1);

            Delete.ForeignKey("FK__RecipeIng__Ingre__6EF57B66").OnTable("RecipeIngredients");
            Create.ForeignKey("FK_RecipeIngredients_Recipes").FromTable("RecipeIngredients").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
            Delete.ForeignKey("FK__RecipeTag__Recip__3E1D39E1").OnTable("RecipeTags");
            Create.ForeignKey("FK_RecipeTag_Recipes").FromTable("RecipeTags").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
            Delete.ForeignKey("FK__Steps__RecipeId__787EE5A0").OnTable("Steps");
            Create.ForeignKey("FK_Steps_Recipes").FromTable("Steps").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id").OnDelete(System.Data.Rule.Cascade);
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_Steps_Recipes").OnTable("Steps");
            Create.ForeignKey("FK__Steps__RecipeId__787EE5A0").FromTable("Steps").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id");
            Delete.ForeignKey("FK_RecipeTag_Recipes").OnTable("RecipeTags");
            Create.ForeignKey("FK__RecipeTag__Recip__3E1D39E1").FromTable("RecipeTags").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id");
            Delete.ForeignKey("FK_RecipeIngredients_Recipes").OnTable("RecipeIngredients");
            Create.ForeignKey("FK__RecipeIng__Ingre__6EF57B66").FromTable("RecipeIngredients").ForeignColumn("RecipeId").ToTable("Recipes").PrimaryColumn("Id");

            Delete.Column("AreMeasurementsStandard").FromTable("Recipes");

            Delete.Column("Description").FromTable("RecipeIngredients");

            Alter.Column("ServingSize").OnTable("Recipes").AsInt16().NotNullable();
        }
    }
}
