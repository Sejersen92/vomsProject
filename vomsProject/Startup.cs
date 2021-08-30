using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.Add(new ServiceDescriptor(typeof(StorageHelper), (provider)
                => new StorageHelper(Configuration.GetConnectionString("BlobStorageConnection"), Configuration["BlobStorageName"]), ServiceLifetime.Scoped));
            services.AddScoped(typeof(DatabaseHelper));
            services.AddScoped(typeof(SolutionHelper));
            services.AddScoped(typeof(DomainHelper));
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseWhen(isHostRootDomain, (app) =>
            {
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
                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
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
