using Microsoft.EntityFrameworkCore;
// using Projectr.Core.Entities;

namespace Projectr.Infrastructure.Data.SQL
{
    public class ProjectrContext : DbContext
    {
        public ProjectrContext(DbContextOptions<ProjectrContext> options) : base(options) { }

        // DbSet for each entity
        public required DbSet<Note> Notes { get; set; }
        public required DbSet<Tag> Tags { get; set; }
        public required DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Additional configurations if needed
            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tags);

            modelBuilder.Entity<Note>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }

    public class Note
    {
        public int NoteId { get; set; }

        public required AppUser Owner { get; set; }
        public required string Title { get; set; }
        public string? MarkdownContent { get; set; }
        public required DateTime CreatedAt { get; set; }
        public List<Tag> Tags { get; } = new();
    }

    public class AppUser
    {
        public int AppUserId { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }
    }

    public class Tag
    {
        public int TagId { get; set; }
        public required string TagLabel { get; set; }
    }
}