﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using vomsProject.Data;

namespace vomsProject.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("vomsProject.Data.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PageId")
                        .HasColumnType("int");

                    b.Property<int?>("SolutionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PageId");

                    b.HasIndex("SolutionId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("vomsProject.Data.Layout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FooterContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FooterEditableContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HeaderContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HeaderEditableContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SaveDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SavedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("SolutionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SavedById");

                    b.HasIndex("SolutionId");

                    b.ToTable("Layouts");
                });

            modelBuilder.Entity("vomsProject.Data.Page", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("HtmlRenderContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("bit");

                    b.Property<int?>("LastSavedVersionId")
                        .HasColumnType("int");

                    b.Property<int?>("LayoutId")
                        .HasColumnType("int");

                    b.Property<string>("PageName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PublishedVersionId")
                        .HasColumnType("int");

                    b.Property<int?>("SolutionId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LastSavedVersionId")
                        .IsUnique()
                        .HasFilter("[LastSavedVersionId] IS NOT NULL");

                    b.HasIndex("LayoutId");

                    b.HasIndex("PublishedVersionId")
                        .IsUnique()
                        .HasFilter("[PublishedVersionId] IS NOT NULL");

                    b.HasIndex("SolutionId");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("vomsProject.Data.PageContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PageId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SaveDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SavedById")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("PageId");

                    b.HasIndex("SavedById");

                    b.ToTable("PageContents");
                });

            modelBuilder.Entity("vomsProject.Data.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PermissionLevel")
                        .HasColumnType("int");

                    b.Property<int?>("SolutionId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SolutionId");

                    b.HasIndex("UserId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("vomsProject.Data.Solution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Domain")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StyleId")
                        .HasColumnType("int");

                    b.Property<string>("Subdomain")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("StyleId");

                    b.ToTable("Solutions");
                });

            modelBuilder.Entity("vomsProject.Data.Style", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Css")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Styles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Css = "* \r\n                        {box-sizing: border-box;} \r\n                            body {font-family: sans-serif;}",
                            Name = "Style 1"
                        });
                });

            modelBuilder.Entity("vomsProject.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Nickname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<int>("ProductVersion")
                        .HasColumnType("int");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("vomsProject.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("vomsProject.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("vomsProject.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("vomsProject.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("vomsProject.Data.Image", b =>
                {
                    b.HasOne("vomsProject.Data.Page", "Page")
                        .WithMany()
                        .HasForeignKey("PageId");

                    b.HasOne("vomsProject.Data.Solution", "Solution")
                        .WithMany("Images")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Page");

                    b.Navigation("Solution");
                });

            modelBuilder.Entity("vomsProject.Data.Layout", b =>
                {
                    b.HasOne("vomsProject.Data.User", "SavedBy")
                        .WithMany()
                        .HasForeignKey("SavedById");

                    b.HasOne("vomsProject.Data.Solution", null)
                        .WithMany("Layouts")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("SavedBy");
                });

            modelBuilder.Entity("vomsProject.Data.Page", b =>
                {
                    b.HasOne("vomsProject.Data.PageContent", "LastSavedVersion")
                        .WithOne()
                        .HasForeignKey("vomsProject.Data.Page", "LastSavedVersionId");

                    b.HasOne("vomsProject.Data.Layout", "Layout")
                        .WithMany()
                        .HasForeignKey("LayoutId");

                    b.HasOne("vomsProject.Data.PageContent", "PublishedVersion")
                        .WithOne()
                        .HasForeignKey("vomsProject.Data.Page", "PublishedVersionId");

                    b.HasOne("vomsProject.Data.Solution", "Solution")
                        .WithMany("Pages")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("LastSavedVersion");

                    b.Navigation("Layout");

                    b.Navigation("PublishedVersion");

                    b.Navigation("Solution");
                });

            modelBuilder.Entity("vomsProject.Data.PageContent", b =>
                {
                    b.HasOne("vomsProject.Data.Page", "Page")
                        .WithMany("Versions")
                        .HasForeignKey("PageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("vomsProject.Data.User", "SavedBy")
                        .WithMany()
                        .HasForeignKey("SavedById");

                    b.Navigation("Page");

                    b.Navigation("SavedBy");
                });

            modelBuilder.Entity("vomsProject.Data.Permission", b =>
                {
                    b.HasOne("vomsProject.Data.Solution", "Solution")
                        .WithMany("Permissions")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("vomsProject.Data.User", "User")
                        .WithMany("Permissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Solution");

                    b.Navigation("User");
                });

            modelBuilder.Entity("vomsProject.Data.Solution", b =>
                {
                    b.HasOne("vomsProject.Data.Style", "Style")
                        .WithMany()
                        .HasForeignKey("StyleId");

                    b.Navigation("Style");
                });

            modelBuilder.Entity("vomsProject.Data.Page", b =>
                {
                    b.Navigation("Versions");
                });

            modelBuilder.Entity("vomsProject.Data.Solution", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Layouts");

                    b.Navigation("Pages");

                    b.Navigation("Permissions");
                });

            modelBuilder.Entity("vomsProject.Data.User", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
