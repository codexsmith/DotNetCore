using Microsoft.AspNetCore.Mvc;
using Projectr.Infrastructure.Data.Neo4j;

namespace Projectr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly Neo4jService _neo4jService;

        public NotesController(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        [HttpGet("tag/{tagName}")]
        public async Task<IActionResult> GetNotesByTag(string tagName)
        {
            var notes = await _neo4jService.GetNotesByTagAsync(tagName);
            return Ok(notes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
        {
            await _neo4jService.CreateNoteAsync(request.Id, request.Title, request.Markdown, DateTime.UtcNow);
            return Ok();
        }

        [HttpGet("{noteId}/linked")]
        public async Task<IActionResult> GetLinkedNotes(string noteId)
        {
            var linkedNotes = await _neo4jService.GetLinkedNotesAsync(noteId);
            return Ok(linkedNotes);
        }
    }

    public class CreateNoteRequest
    {
        public required string Id { get; set; }
        public required string Title { get; set; }
        public required string Markdown { get; set; }
    }
}
