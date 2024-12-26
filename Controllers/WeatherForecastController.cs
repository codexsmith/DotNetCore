using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Projectr.Infrastructure.Data;

namespace DotNetCore.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDriver _neo4j;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IDriver driver)
    {
        _logger = logger;
        _neo4j = driver;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    public async Task<List<string>> GetNotes()
    {
        var names = new List<string>();

        using var session = _neo4j.AsyncSession();
        // var result = await session.RunAsync("MATCH (n) RETURN n.name AS name");
        var cursor = await session.RunAsync("MATCH (n) RETURN n.name AS name");

        // Iterate over each record in the result set
        while (await cursor.FetchAsync())
        {
            names.Add(cursor.Current["name"].As<string>());
        }

        return names;
    }
}
