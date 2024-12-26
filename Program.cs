using Microsoft.EntityFrameworkCore;
using Projectr.Infrastructure.Data.SQL;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var neo4jConfig = configuration.GetSection("Neo4j");

// Extract Neo4j connection details
// var neo4jUrl = neo4jConfig["Host"] ?? "NoHost";
// var neo4jUsername = neo4jConfig["Username"] ?? "NoUserName";
// var neo4jPassword = neo4jConfig["Password"] ?? "NoPassword";

// Add services to the container.
builder.Services.AddDbContext<ProjectrContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// builder.Services.AddSingleton(GraphDatabase.Driver(
//     neo4jUrl, AuthTokens.Basic(neo4jUsername, neo4jPassword)));

// builder.Services.AddIdentity<ProjectrUser, ProjectrUserRole>()
//     .AddUserStore<NeoUserStore>()
//     .AddRoleStore<NeoUserRoleStore>()
//     .AddDefaultTokenProviders();

// Register Neo4jService
// builder.Services.AddSingleton(provider =>
//     new Neo4jService(neo4jUrl, neo4jUsername, neo4jPassword));
//     "bolt://localhost:7687", // Replace with your Neo4j URI
//     "ProjectrAdmin",              // Replace with your Neo4j username
//     "Geni123191"               // Replace with your Neo4j password
// ));

// if (args.Length > 0 && args[0].ToLower() == "migrate")
// {
//     NeoMigrations.Run(builder.Configuration, builder.Environment.ContentRootPath);
//     Console.WriteLine("returning from migration routine");
//     return;
// }

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
