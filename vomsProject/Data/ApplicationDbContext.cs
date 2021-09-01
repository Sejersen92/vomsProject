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
            modelBuilder.Entity<Page>()
                .HasOne(p => p.LastSavedVersion);
            modelBuilder.Entity<Page>()
                .HasOne(p => p.PublishedVersion);
            modelBuilder.Entity<Page>()
                .HasMany(p => p.Versions)
                .WithOne(c => c.Page);

            //using (var style1 = new System.IO.StreamReader("Data/SeedData/Styles/style1.css"))
            //{
            //    modelBuilder.Entity<Style>().HasData(new Style
            //    {
            //        Id = 1,
            //        Name = "Style 1",
            //        Css = style1.ReadToEnd()
            //    });
            //}
        }
    }
}
