using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.EfCore;
using ShopApp.WEBUI.EmailServices;
using ShopApp.WEBUI.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WEBUI
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data Source=shopDb"));
            services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //password
                options.Password.RequireDigit= true;
                options.Password.RequireLowercase= true;
                options.Password.RequireUppercase= true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric= true;

                //Lockout
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers= true;

                //options.User.AllowedUserNameCharacters = "";
                options.User.RequireUniqueEmail= true;
                options.SignIn.RequireConfirmedEmail= true;
                options.SignIn.RequireConfirmedPhoneNumber= false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan= TimeSpan.FromMinutes(60);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name =".ShopApp.Security.Cookie",
                    SameSite= SameSiteMode.Strict
                };
            });

            services.AddScoped<IProductRepository, EfCoreProductRepository>();          
            services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
            services.AddScoped<ICartRepository, EfCoreCartRepository>();

            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICartService, CartManager>();

            services.AddScoped<IEmailSender, SmtpEmailSender>(i => new SmtpEmailSender(
                _configuration["EmailSender:Host"],
                _configuration.GetValue<int>("EmailSender:Port"),
                _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                _configuration["EmailSender:UserName"],
                _configuration["EmailSender:Password"]
                )
            );

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "checkout",
                  pattern: "/checkout",
                  defaults: new { controller = "Cart", Action = "Checkout" }
                  );

                endpoints.MapControllerRoute(
                   name: "cart",
                   pattern: "cart",
                   defaults: new { controller = "Cart", Action = "Index" }
                   );

                endpoints.MapControllerRoute(
                   name: "adminroles",
                   pattern: "admin/role/list",
                   defaults: new { controller = "Admin", Action = "RoleList" }
                   );

                endpoints.MapControllerRoute(
                   name: "adminrolecreate",
                   pattern: "admin/role/create",
                   defaults: new { controller = "Admin", Action = "RoleCreate" }
                   );

                endpoints.MapControllerRoute(
                  name: "adminroleedit",
                  pattern: "admin/role/{id?}",
                  defaults: new { controller = "Admin", Action = "RoleEdit" }
                  );

                endpoints.MapControllerRoute(
                   name: "adminproducts",
                   pattern: "admin/products",
                   defaults: new { controller = "Admin", Action = "Productlist" }
                   );

                endpoints.MapControllerRoute(
                    name: "adminproductcreate",
                    pattern: "admin/products/create",
                    defaults: new { controller = "Admin", Action = "ProductCreate" }
                    );

                endpoints.MapControllerRoute(
                   name: "adminproductedit",
                   pattern: "admin/products/{id?}",
                   defaults: new { controller = "Admin", Action = "ProductEdit" }
                   );

                endpoints.MapControllerRoute(
                    name: "admincategories",
                    pattern: "admin/categories",
                    defaults: new { controller = "Admin", Action = "Categorylist" }
                    );

                endpoints.MapControllerRoute(
                    name: "admincategorycreate",
                    pattern: "admin/categories/create",
                    defaults: new { controller = "Admin", Action = "CategoryCreate" }
                    );               

                endpoints.MapControllerRoute(
                  name: "admincategorytedit",
                  pattern: "admin/categories/{id?}",
                  defaults: new { controller = "Admin", Action = "CategoryEdit" }
                  );

                endpoints.MapControllerRoute(
                    name: "products",
                    pattern: "products/{category?}",
                    defaults: new { controller = "Shop", Action = "list" }
                    );

                endpoints.MapControllerRoute(
                    name: "search",
                    pattern: "search",
                    defaults: new { controller = "Shop", Action = "search" }
                    );

                endpoints.MapControllerRoute(
                    name: "Productdetails",
                    pattern: "{url}",
                    defaults: new { controller = "Shop", Action = "detail" }
                    );
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                ); 
            });
        }
    }
}
