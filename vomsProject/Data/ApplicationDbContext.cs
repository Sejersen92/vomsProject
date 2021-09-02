using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace vomsProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<Layout> Layouts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Style> Styles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Solution>()
                .HasMany(s => s.Pages)
                .WithOne(p => p.Solution)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Solution>()
                .HasMany(s => s.Permissions)
                .WithOne(p => p.Solution)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Solution>()
                .HasMany(s => s.Images)
                .WithOne(i => i.Solution)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Solution>()
               .HasMany(s => s.Layouts)
               .WithOne()
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Page>()
                .HasOne(p => p.LastSavedVersion)
                .WithOne();
            modelBuilder.Entity<Page>()
                .HasOne(p => p.PublishedVersion)
                .WithOne();
            modelBuilder.Entity<Page>()
                .HasMany(p => p.Versions)
                .WithOne(c => c.Page)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Permissions)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Style>().HasData(new Style
            {
                Id = 1,
                Name = "Style 1",
                Css = @"* 
                        {box-sizing: border-box;} 
                            body {font-family: sans-serif;}"
            });
        }
    }
}
