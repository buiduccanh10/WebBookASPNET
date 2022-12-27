using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBookk.Areas.Identity.Data;
using WebBookk.Data;

namespace WebBookk
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
            var builder = WebApplication.CreateBuilder();
            
            var connectionString1 = builder.Configuration.GetConnectionString("DefaultMYSQLConnection");
            var connectionString2 = builder.Configuration.GetConnectionString("IdentityMYSQLConnection");
            // Config MSSQL Server
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddDbContext<WebBookIdentityDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("IdentityConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();

            // Config MySQL 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString1, ServerVersion.AutoDetect(connectionString1)));
            services.AddDatabaseDeveloperPageExceptionFilter();
            
            services.AddDbContext<WebBookkIdentityDbContext>(options =>
                options.UseMySql(
                    connectionString2, ServerVersion.AutoDetect(connectionString2)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();
            //services.AddScoped<IDataFunctions, DataFunctions>();
            services.AddMvc()
            .AddSessionStateTempDataProvider();
            services.AddSession();
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "Admin",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
