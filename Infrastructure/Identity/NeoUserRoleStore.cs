using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Neo4j.Driver;
using Projectr.Models.Identity;


namespace Projectr.Infrastructure.Identity.Neo
{
    public class NeoUserRoleStore : IRoleStore<ProjectrUserRole>
    {
        private readonly IDriver _neo4jDriver;

        public NeoUserRoleStore(IDriver neo4jDriver)
        {
            _neo4jDriver = neo4jDriver;
        }

        public async Task<IdentityResult> CreateAsync(ProjectrUserRole role, CancellationToken cancellationToken)
        {
            using var session = _neo4jDriver.AsyncSession();
            var query = @"
                CREATE (r:Role {Id: $id, Name: $name, NormalizedName: $normalizedName})
                RETURN r";
            await session.RunAsync(query, new
            {
                id = role.Id,
                name = role.Name,
                normalizedName = role.NormalizedName
            });

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ProjectrUserRole role, CancellationToken cancellationToken)
        {
            using var session = _neo4jDriver.AsyncSession();
            var query = "MATCH (r:Role {Id: $id}) DETACH DELETE r";
            await session.RunAsync(query, new { id = role.Id });

            return IdentityResult.Success;
        }

        public Task<ProjectrUserRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            // Implement similar to Neo4jUserStore
            throw new NotImplementedException();
        }

        public Task<ProjectrUserRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            // Implement similar to Neo4jUserStore
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(ProjectrUserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string?> GetRoleNameAsync(ProjectrUserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(ProjectrUserRole role, string? roleName, CancellationToken cancellationToken)
        {
            if (role.NormalizedName == null || roleName == null)
            {
                throw new InvalidOperationException("NormalizedUserName cannot be null.");
            }
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedRoleNameAsync(ProjectrUserRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(ProjectrUserRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            if (role.NormalizedName == null || normalizedName == null)
            {
                throw new InvalidOperationException("NormalizedUserName cannot be null.");
            }
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose resources if necessary
        }

        public async Task<IdentityResult> UpdateAsync(ProjectrUserRole role, CancellationToken cancellationToken)
        {
            try
            {
                using var session = _neo4jDriver.AsyncSession();
                var query = @"
            MATCH (r:Role {Id: $id})
            SET r.Name = $name,
                r.NormalizedName = $normalizedName
            RETURN r";

                var parameters = new
                {
                    id = role.Id,
                    name = role.Name,
                    normalizedName = role.NormalizedName
                };

                var result = await session.RunAsync(query, parameters);

                // Fetch the single record to verify the update
                if (await result.FetchAsync())
                {
                    // A record exists, indicating a successful update
                    return IdentityResult.Success;
                }

                // If no record was returned, the update likely failed
                return IdentityResult.Failed(new IdentityError { Description = "Role not found or update failed." });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return IdentityResult.Failed(new IdentityError { Description = $"An error occurred: {ex.Message}" });
            }
        }

    }
}
