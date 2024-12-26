using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver;
using Projectr.Models.Identity;


namespace Projectr.Infrastructure.Identity.Neo
{
    public class NeoUserStore : IUserStore<ProjectrUser>, IUserPasswordStore<ProjectrUser>
    {
        private readonly IDriver _neo4jDriver;

        public NeoUserStore(IDriver neo4jDriver)
        {
            _neo4jDriver = neo4jDriver;
        }

        public async Task<IdentityResult> CreateAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            using var session = _neo4jDriver.AsyncSession();
            var query = @"
                CREATE (u:User {Id: $id, UserName: $username, Email: $email, PasswordHash: $passwordHash})
                RETURN u";
            await session.RunAsync(query, new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                passwordHash = user.PasswordHash
            });

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            using var session = _neo4jDriver.AsyncSession();
            var query = "MATCH (u:User {Id: $id}) DETACH DELETE u";
            await session.RunAsync(query, new { id = user.Id });

            return IdentityResult.Success;
        }

        public async Task<ProjectrUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using var session = _neo4jDriver.AsyncSession();
            var query = "MATCH (u:User {Id: $id}) RETURN u";
            var result = await session.RunAsync(query, new { id = userId });

            INode? userNode = null;

            if (await result.FetchAsync())
            {
                var record = result.Current; // Access the current record
                userNode = record["u"].As<INode>(); // Get the 'u' value as a node
                Console.WriteLine($"User: {userNode.Properties["name"]}");
            }
            else
            {
                Console.WriteLine("No user found with the specified email.");
            }

            if (userNode == null) return null;

            return new ProjectrUser
            {
                Id = userNode["Id"].As<string>(),
                UserName = userNode["UserName"].As<string>(),
                Email = userNode["Email"].As<string>(),
                PasswordHash = userNode["PasswordHash"].As<string>()
            };
        }

        public async Task<ProjectrUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using var session = _neo4jDriver.AsyncSession();
            var query = "MATCH (u:User {UserName: $username}) RETURN u";
            var result = await session.RunAsync(query, new { username = normalizedUserName });

            INode? userNode = null;

            if (await result.FetchAsync())
            {
                var record = result.Current; // Access the current record
                userNode = record["u"].As<INode>(); // Get the 'u' value as a node
                Console.WriteLine($"User: {userNode.Properties["name"]}");
            }
            else
            {
                Console.WriteLine("No user found with the specified email.");
            }

            if (userNode == null) return null;

            return new ProjectrUser
            {
                Id = userNode["Id"].As<string>(),
                UserName = userNode["UserName"].As<string>(),
                Email = userNode["Email"].As<string>(),
                PasswordHash = userNode["PasswordHash"].As<string>()
            };
        }

        public Task<string?> GetPasswordHashAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(ProjectrUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }

        public Task<string?> GetNormalizedUserNameAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ProjectrUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ProjectrUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ProjectrUser user, CancellationToken cancellationToken)
        {
            try
            {
                using var session = _neo4jDriver.AsyncSession();
                var query = @"
            MATCH (u:User {Id: $id})
            SET u.UserName = $username,
                u.Email = $email,
                u.PasswordHash = $passwordHash,
                u.NormalizedUserName = $normalizedUserName,
                u.NormalizedEmail = $normalizedEmail
            RETURN u";

                var parameters = new
                {
                    id = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    passwordHash = user.PasswordHash,
                    normalizedUserName = user.NormalizedUserName,
                    normalizedEmail = user.NormalizedEmail
                };

                var result = await session.RunAsync(query, parameters);

                // Fetch the single record if it exists
                if (await result.FetchAsync())
                {
                    // A record exists, indicating a successful update
                    return IdentityResult.Success;
                }

                // If no record was returned, the update likely failed
                return IdentityResult.Failed(new IdentityError { Description = "User not found or update failed." });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return IdentityResult.Failed(new IdentityError { Description = $"An error occurred: {ex.Message}" });
            }
        }

    }
}
