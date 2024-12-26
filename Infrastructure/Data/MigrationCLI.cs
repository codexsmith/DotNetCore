using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Projectr.Infrastructure.Migrations.Neo;

public class NeoMigrations
{
    public static void Run(IConfiguration configuration, string projectRoot)
    {
        var neo4jConfig = configuration.GetSection("Neo4j");
        var url = neo4jConfig["Host"];
        var username = neo4jConfig["Username"];
        var password = neo4jConfig["Password"];

        var migrationExecutable = Path.Combine(projectRoot, "bin", "neo4j-migrations.exe");
        var migrationFolder = Path.Combine(projectRoot, "NeoMigrations");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = migrationExecutable,
                Arguments = $"-a {url} --username {username} --password {password} --location file:///{migrationFolder.Replace("\\", "/")} migrate",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        Console.WriteLine($"{migrationExecutable}");
        Console.WriteLine($"{process.StartInfo.Arguments}");
        Console.WriteLine("Starting Migrations");
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine("Migration succeeded:");
            Console.WriteLine(output);
        }
        else
        {
            Console.WriteLine("Migration failed:");
            Console.WriteLine(error);
            throw new Exception($"Migration process exited with code {process.ExitCode}. \\n Error {error}. \\n Output {output}");
        }
    }
}