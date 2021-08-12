using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebBanking.BackgroundServices;
using WebBanking.Data;
using WebBanking.Models;
using WebBanking.Repository;

namespace WebBanking
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
            services.AddControllersWithViews();

            // Add database connection string
            services.AddDbContext<WebBankContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("WebBankContext"));
                // Enable lazy loading.
                options.UseLazyLoadingProxies();
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "1234567890";
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

            }
            ).AddEntityFrameworkStores<WebBankContext>();

            // Enable Background Services
            services.AddHostedService<BillPayBackgroundService>();


            //// Store session into Web-Server memory.


            //services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    // Make the session cookie essential.
            //    options.Cookie.IsEssential = true;
            //});



            /* Store session into SQL Server.
             * Package required: Microsoft.Extensions.Caching.SqlServer 
             * 
             * before using, run the following commands:
             * 
             * # Installing tools
             * dotnet tool install --global dotnet-sql-cache
             * OR
             * dotnet tool update --global dotnet-sql-cache
             * 
             * # Distributed SQL Server Cache
             * Note the schema used below is dotnet not the default schema dbo.
             * 
             * The dotnet schema will need to be created on the database in advance with this SQL:
             * 
             * create schema dotnet;
             * 
             * Run the following command to create the session cache table:
             * 
             * dotnet sql-cache create "<connection string>" <schema name> <table name>
             * dotnet sql-cache create "Server=rmit.australiaeast.cloudapp.azure.com;Uid=s3820251_a2;Pwd=abc123;" dotnet SessionCache
             */
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString(nameof(WebBankContext));
                options.SchemaName = "dotnet";
                options.TableName = "SessionCache";
            });
            services.AddSession(options =>
            {
                // Make the session cookie essential.
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromDays(7);
            });

            services.AddScoped<IUserRepository, UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            // enables authentication
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
