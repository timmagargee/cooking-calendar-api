using Migrations.DomainModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using CommandLine;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Migrations
{
    public class Program
    {
        private static ProgramConfig _config;
        private static IServiceProvider _services;

        public static void Main(string[] args)
        {
            LoadConfig();

            CreateServiceCollection();

            var goingUp = false;

            // Parse the commandline arguments if there are any.
            // Example:  Migrations.exe <args>
            // arguments:
            // -u or -up => Migrate up.
            // -d or -down => Migrate down.
            Parser.Default.ParseArguments<CommandLineArgsOptions>(args)
                .WithParsed(o =>
                {
                    goingUp = o.Up;
                });
            if (goingUp)
            {
                UpdateDatabase();
            }
            else
            {
                RevertLastMigration();
            }
            Shutdown();
        }
        private static void LoadConfig()
        {
            _config =new ConfigurationBuilder()
                .AddJsonFile("appsettings.local.json")
                .Build()
                .Get<ProgramConfig>()!;
        }

        private static void CreateServiceCollection()
        {
            _services =
                new ServiceCollection()
                    .AddFluentMigratorCore()
                    .ConfigureRunner(
                        rb =>
                            rb
                                .AddSqlServer()
                                .WithGlobalConnectionString(_config.ConnectionString)
                                .ScanIn(typeof(Program).Assembly).For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole())
                    .BuildServiceProvider();
        }

        private static void UpdateDatabase()
        {
            _services.GetService<IMigrationRunner>()!.ListMigrations();
             _services.GetService<IMigrationRunner>()!.MigrateUp();
        }

        private static void RevertLastMigration()
        {
            _services.GetService<IMigrationRunner>()!.Rollback(1);
        }

        private static void Shutdown()
        {
            Console.Write("Press any key...");
            Console.ReadLine();
        }
    }
}