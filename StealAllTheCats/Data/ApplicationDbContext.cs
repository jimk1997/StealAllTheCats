using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using StealAllTheCats.Models;

namespace StealAllTheCats.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<CatEntity> Cats { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<CatTagEntity> CatTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatTagEntity>()
                .HasKey(ct => new { ct.CatEntityId, ct.TagEntityId });

            modelBuilder.Entity<CatTagEntity>()
                .HasOne(ct => ct.Cat)
                .WithMany(c => c.CatTags)
                .HasForeignKey(ct => ct.CatEntityId);

            modelBuilder.Entity<CatTagEntity>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CatTags)
                .HasForeignKey(ct => ct.TagEntityId);
        }
    }
}