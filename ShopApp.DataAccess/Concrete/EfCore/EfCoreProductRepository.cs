using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ShopContext>, IProductRepository
    {
        public int GetCountByCategory(string category)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.Where(i=>i.IsApproved==true).AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products.Include(i => i.ProductCategories)
                        .ThenInclude(i => i.category)
                        .Where(i => i.ProductCategories.Any(a => a.category.Url == category));
                }
                return products.Count();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products.Where(i => i.IsApproved == true && i.IsHome == true).ToList();
            }
        }    

        public Product GetProductDetails(string url)
        {
            using(var context = new ShopContext())
            {
                return context.Products.Where(i => i.Url == url)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.category)
                    .FirstOrDefault();
            }
        }

        public List<Product> GetProductsByCategory(string name, int page, int pageSize)
        {
            using(var context = new ShopContext())
            {
                var products = context.Products.Where(i=>i.IsApproved==true).AsQueryable();
                if (!string.IsNullOrEmpty(name))
                {
                    products = products.Include(i => i.ProductCategories)
                        .ThenInclude(i => i.category)
                        .Where(i => i.ProductCategories.Any(a => a.category.Url == name));
                }
                return products.Skip((page-1)*(pageSize)).Take(pageSize).ToList();
            }
        }

        public List<Product> GetSearchResult(string SearchString)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.
                    Where(i => i.IsApproved == true && 
                    (i.Name.ToLower().Contains(SearchString.ToLower()) || 
                    i.Description.ToLower().
                    Contains(SearchString.ToLower()))).AsQueryable();
                
                return products.ToList();
            }
        }
    }
}
