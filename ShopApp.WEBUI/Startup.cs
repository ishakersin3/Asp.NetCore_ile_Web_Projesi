using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopApp.Business.Abstract;
using ShopApp.Business.Concrete;
using ShopApp.DataAccess.Abstract;
using ShopApp.DataAccess.Concrete.EfCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.WEBUI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProductRepository, EfCoreProductRepository>();          
            services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();

            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();

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

            app.UseRouting();

            // localhost:5000
            // localhost:5000/home
            // localhost:5000/products/details/2
            // localhost:5000/product/list/3
            // localhost:5000/category/list

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "adminproductlist",
                    pattern: "admin/products",
                    defaults: new { controller = "Admin", Action = "Productlist" }
                    );

                endpoints.MapControllerRoute(
                    name: "adminproductlist",
                    pattern: "admin/products/{id?}",
                    defaults: new { controller = "Admin", Action = "Edit" }
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
                    pattern: "{controller=Home}/{action=index}/{id?}"
                ); 
            });
        }
    }
}
