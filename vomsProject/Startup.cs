using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using vomsProject.Data;
using vomsProject.Helpers;

namespace vomsProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("AzureDbConnectionString")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddScoped((provider) =>
                new BlobServiceClient(Configuration.GetValue<string>("ConnectionStrings:BlobStorageConnection")));
            services.AddScoped((provider) => 
                new CloudBlobContainer(new Uri("https://sejersenstorageaccount.blob.core.windows.net/voms"),
                    new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                        Configuration.GetValue<string>("AccountName"),
                        Configuration.GetValue<string>("AccountKey"))));
            services.AddScoped<RepositoryService>();
            services.AddScoped(typeof(DomainHelper));
            services.AddScoped<OperationsService>();
            services.AddSingleton<JwtService>();
        }
        private bool isHostRootDomain(HttpContext context)
        {
            return context.Request.Host.Host == Configuration["RootDomain"];
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseWhen(isHostRootDomain, (app) =>
            {
                app.UseStaticFiles();
                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
            });

            // Here we specify the middleware that are used for solutions.
            app.UseWhen((context) => !isHostRootDomain(context), (app) =>
            {
                app.UseStaticFiles(new StaticFileOptions() 
                {
                    FileProvider = new FilteredFileProvider(env.WebRootFileProvider, (subpath) => {
                        var segments = subpath.TrimStart('/').Split('/');
                        return segments[0] == "lib" || segments[0] == "css";
                    }),
                    RequestPath = ""
                });
                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "Image",
                        pattern: "/pages/{pageId}/images/{imageId}",
                        defaults: new { controller = "Page", action = "Image" });
                    endpoints.MapControllerRoute(
                        name: "Style",
                        pattern: "/favicon.{ext}",
                        defaults: new { controller = "Page", action = "Favicon" });
                    endpoints.MapControllerRoute(
                        name: "Style",
                        pattern: "/Style.css",
                        defaults: new { controller = "Page", action = "Style" });
                    endpoints.MapControllerRoute(
                        name: "Logout",
                        pattern: "/Logout",
                        defaults: new { controller = "Page", action = "Logout" });
                    endpoints.MapControllerRoute(
                        name: "Login",
                        pattern: "/Login",
                        defaults: new { controller = "Page", action = "Login" });
                    endpoints.MapControllerRoute(
                        name: "Page",
                        pattern: "/{*pageName}",
                        defaults: new { controller = "Page", action = "Index" });
                });
            });
        }
    }
}
