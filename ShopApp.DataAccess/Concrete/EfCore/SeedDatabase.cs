using Microsoft.EntityFrameworkCore;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context.Categories.Count()==0)
                {
                    context.Categories.AddRange(Categories);
                }
                if (context.Products.Count() == 0)
                {
                    context.Products.AddRange(Products);
                    context.AddRange(ProductCategories);
                }
                context.SaveChanges();
            }
                        
        }
        private static Category[] Categories =
        {
            new Category(){Name="Telefon",Url="telefon"},
            new Category(){Name="Bilgisayar",Url="bilgisayar"},
            new Category(){Name="Elektronik",Url="elektronik"},
            new Category(){Name="Beyaz Eşya",Url="beyaz-esya"}
        };
        private static Product[] Products =
        {
            new Product(){Name="Samsung S5",Url="samsung-s5",Price=2000,ImageUrl="1.jpg",Description="İyi Telefon",IsApproved=true},           
            new Product(){Name="Samsung S6",Url="samsung-s6",Price=3000,ImageUrl="2.jpg",Description="İyi Telefon",IsApproved=false},           
            new Product(){Name="Samsung S7",Url="samsung-s7",Price=4000,ImageUrl="3.jpg",Description="İyi Telefon",IsApproved=true},           
            new Product(){Name="Samsung S8",Url="samsung-s8",Price=5000,ImageUrl="4.jpg",Description="İyi Telefon",IsApproved=false},           
            new Product(){Name="Samsung S9",Url="samsung-s9",Price=6000,ImageUrl="5.jpg",Description="İyi Telefon",IsApproved=true}          
        };
        private static ProductCategory[] ProductCategories =
        {
            new ProductCategory(){product = Products[0],category=Categories[0]},
            new ProductCategory(){product = Products[0],category=Categories[2]},
            new ProductCategory(){product = Products[1],category=Categories[0]},
            new ProductCategory(){product = Products[1],category=Categories[2]},
            new ProductCategory(){product = Products[2],category=Categories[0]},
            new ProductCategory(){product = Products[2],category=Categories[2]},
            new ProductCategory(){product = Products[3],category=Categories[0]},
            new ProductCategory(){product = Products[3],category=Categories[2]},
        };
    }
}
