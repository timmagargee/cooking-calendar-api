using CookingCalendarApi.Logging;
using CookingCalendarApi.Repositories;
using CookingCalendarApi.StartupClasses;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
Log.Information("Starting host...");


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);
builder.Configuration.AddEnvironmentVariables();

var appConfig = builder.Configuration.Get<AppConfig>()!;

// Remove default logging providers
builder.Logging.ClearProviders();

// Register Serilog
builder.Logging.AddSerilog(LoggingConfig.BuildLogger(appConfig).ForContext<Program>());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvc()
    .AddJsonOptions(opt =>
    {
        if (opt.JsonSerializerOptions != null)
        {
            opt.JsonSerializerOptions.IncludeFields = true;
            opt.JsonSerializerOptions.MaxDepth = 32;
        }
    });

//UNSURE IF NEEDED 
builder.Services.AddCors(options =>
{
    options.AddPolicy("API Policy",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition")
            .Build()
    );
});
builder.Services.AddHttpClient();

builder.Services.AddSingleton(appConfig);

builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<IShoppingRepository, ShoppingRepository>();

//builder.Services.AddAuthentication();
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = "";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("API Policy");

app.UseHttpsRedirection();



//app.UseAuthorization();

app.MapControllers();

app.Run();
