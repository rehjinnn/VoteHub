using Microsoft.EntityFrameworkCore;
using VoteHub.Models;

namespace VoteHub.Data
{
    public class VoteHubContext : DbContext
    {
        public VoteHubContext(DbContextOptions<VoteHubContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            // User configuration
            modelBuilder.Entity<User>()
                .HasMany(u => u.Elections)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.UserId)
                .IsRequired(false)  // ← ADD THIS LINE
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Votes)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Election configuration
            modelBuilder.Entity<Election>()
                .HasMany(e => e.Candidates)
                .WithOne(c => c.Election)
                .HasForeignKey(c => c.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Election>()
                .HasMany(e => e.Votes)
                .WithOne(v => v.Election)
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Candidate configuration
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.Votes)
                .WithOne(v => v.Candidate)
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vote unique constraint
            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.ElectionId, v.UserId })
                .IsUnique();
        }
    }
}