using Neo4j.Driver;

namespace Projectr.Infrastructure.Data.Neo4j
{
    public class Neo4jService : IDisposable
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        public async Task<List<string>> GetNotesByTagAsync(string tagName)
        {
            var notes = new List<string>();

            using var session = _driver.AsyncSession();
            var query = @"
                MATCH (n:Note)-[:TAGGED_WITH]->(t:Tag {name: $tagName})
                RETURN n.title AS title";

            var result = await session.RunAsync(query, new { tagName });
            while (await result.FetchAsync())
            {
                notes.Add(result.Current["title"].As<string>());
            }

            return notes;
        }

        public async Task CreateNoteAsync(string noteId, string title, string markdown, DateTime createdAt)
        {
            using var session = _driver.AsyncSession();
            var query = @"
                CREATE (n:Note {id: $id, title: $title, markdown: $markdown, created_at: $createdAt})";
            await session.RunAsync(query, new { id = noteId, title, markdown, createdAt });
        }

        public async Task<List<string>> GetLinkedNotesAsync(string noteId)
        {
            var linkedNotes = new List<string>();

            using var session = _driver.AsyncSession();
            var query = @"
                MATCH (n:Note {id: $noteId})-[:LINKED_TO]-(m:Note)
                RETURN m.title AS title";

            var result = await session.RunAsync(query, new { noteId });
            while (await result.FetchAsync())
            {
                linkedNotes.Add(result.Current["title"].As<string>());
            }

            return linkedNotes;
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
