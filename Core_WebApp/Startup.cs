using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Core_WebApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Core_WebApp.Models;
using Core_WebApp.Services;
using Core_WebApp.CustomFilters;
using Microsoft.OpenApi.Models;
using Core_WebApp.Middlewares;

namespace Core_WebApp
{
    public class Startup
    {
        /// <summary>
        /// IConfiguration : Load the appsettings.json
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// This method provides a Depednency Container for
        /// Registering and resolving external objects required for the
        /// application. These objects are
        /// DbContext, Custom Business Services, Security (Authentication/Authorization)
        /// CORS, JWT, MVC Controllers, Razor Pages, etc.
        /// This also define Lifectime for the Object using
        /// using ServiceLifetime enumeration with values as
        /// 1. Singleton --> One single object throught the applicaiton lifetime  
        /// 2. Scoped --> One object per session (Stateful)
        /// 3. Transient --> An object for a specific request (Stateless)
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // The connection  and Data Access management using EF Core
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            // register the AppJune2020DbContext class in Dependency Container
            // the instance for AppJune2020DbContext will be injected in class
            // using Constructor injection
            services.AddDbContext<AppJune2020DbContext>(
                  options => options.UseSqlServer(
                       Configuration.GetConnectionString("AppDbConnection")
                      )
                );

            // The Identityt Management for Authentication
            // SignIn.RequireConfirmedAccount = true
            // Verify the Email
            // Used for only 'UserManager<IdentityUser>' resolve
            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            // Resolve follwoing classsed from DI
            // UserManager<IdentityUser>
            // RoleManager<IdentityRole>
            services.AddIdentity<IdentityUser,IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>();

            // configure the CORS service
            services.AddCors(options => {
                options.AddPolicy("corspolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            // ends here

            // configure the swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", 
                    new OpenApiInfo { Title = "ASP.NET Core API", Version = "v1" });
            });

            // ends here


            // define policies

            services.AddAuthorization(options => {

                options.AddPolicy("readpolicy", policy =>
                {
                    policy.RequireRole("Manager", "Clerk", "Operator"); 
                });

                options.AddPolicy("writepolicy", policy =>
                {
                    policy.RequireRole("Manager", "Clerk");
                });
            });

            // add the distributed memory cache service and session
            services.AddDistributedMemoryCache();
            services.AddSession(session => {
                session.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            // ends

            // register repository classes in DI Container
            services.AddScoped<IRepository<Category, int>, CategoryRepository>();
            services.AddScoped<IRepository<Product, int>, ProductRepository>();
            // The MVC COntroller and View Request Procvessing
            // configuring the Filters
            // AddControllersWithViews --> used for processing MVC Controller with views
            // and API Controllers
            services.AddControllersWithViews(options => {
              // options.Filters.Add(new LogFilterAttribute());
               // options.Filters.Add(typeof(BusinessExceptionFilter));
            });
            // used for processing the WEB API Controller
            services.AddControllers()
                  .AddJsonOptions(options => {
                      options.JsonSerializerOptions.PropertyNamingPolicy = null;
                  }); 
            // The Razor Pages Execution (Need for the Indentity Pages e.g. register/login)
            services.AddRazorPages();
        }

        // This method gets called by the runtime. 
        //Use this method to configure the HTTP request pipeline.
        // Register all Middlewares for execution
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            // reda all .js/.css files from wwwroot folder and respond to
            // thesse files to browser
            app.UseStaticFiles();

            // create routing table and verify the route expression
            app.UseRouting();
            app.UseSession(); // the session

            // configure the CORS middleware
            app.UseCors("corspolicy");

            // configure the swagger middleware
            app.UseSwagger();

            // provide / respond the HTML UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core API Documentation");
            });


            // security
            app.UseAuthentication(); // user management to verify user
            app.UseAuthorization(); // role management to verify roles

            // Register the Custom Exception Middleware
            app.CustomExceptionMiddleware();

            // publish the application on Host http Endpoint
            app.UseEndpoints(endpoints =>
            {
                // Map the incomming request with the Route table
                // to load and execute MVC Controller
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                // used in case of Web Forms (Uased for Indentity pages)
                endpoints.MapRazorPages();
            });
        }
    }
}
