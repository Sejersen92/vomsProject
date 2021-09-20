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
            modelBuilder.Entity<Solution>()
               .HasOne(s => s.DefaultLayout)
               .WithOne()
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Solution>()
                .Property(x => x.StylesheetCustomization)
                .IsRequired();

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
                Css = @"*{
                        box-sizing: border-box;
                        }
                        body{
                        }
                        h1{
                            color: var(--header1Color);
                            font-family: var(--header1Font);
                        }
                        h2{
                            color: var(--header2Color);
                            font-family: var(--header2Font);
                        }
                        h3{
                            color: var(--header3Color);
                            font-family: var(--header3Font);
                        }
                        h4{
                            color: var(--header4Color);
                            font-family: var(--header4Font);
                        }
                        h5{
                            color: var(--header5Color);
                            font-family: var(--header5Font);
                        }
                        h6{
                            color: var(--header6Color);
                            font-family: var(--header6Font);
                        }
                        p{
                            color: var(--paragraphColor);
                            font-family: var(--paragraphFont);
                        }
                        b{
	                        color: var(--boldColor);
	                        font-family: var(--boldFont)
                        }
                        em{
	                        color: var(--emphasisColor);
	                        font-family: var(--emphasisFont)
                        }",
                StylesheetOptions =
                        @"
                        header1Color,The color of the h1 element,color
                        header1Font,The font of the h1 element,font
                        header2Color,The color of the h2 element,color
                        header2Font,The font of the h2 element,font
                        header3Color,The color of the h3 element,color
                        header3Font,The font of the h3 element,font
                        header4Color,The color of the h4 element,color
                        header4Font,The font of the h4 element,font
                        header5Color,The color of the h5 element,color
                        header5Font,The font of the h5 element,font
                        header6Color,The color of the h6 element,color
                        header6Font,The font of the h6 element,font
                        paragraphColor,The color of the paragraph element,color
                        paragraphFont,The font of the paragraph element,font
                        boldColor,The color of the bold element,color
                        boldFont,The font of the bold element,font
                        emphasisColor,The color of the emphasis element,color
                        emphasisFont,The font of the h6 element,font
                        "
            });
            modelBuilder.Entity<Style>().Property(x => x.StylesheetOptions).IsRequired();
        }
    }
}
