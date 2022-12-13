using Microsoft.EntityFrameworkCore;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ShopContext>, IProductRepository
    {
        public List<Product> GetPopulerProducts()
        {
            using(var context = new ShopContext())
            {
                return context.Products.ToList();   
            }
        }

        public Product GetProductDetails(int id)
        {
            using(var context = new ShopContext())
            {
                return context.Products
                                .Where(i => i.ProductId == id)
                                .Include(i => i.ProductCategories)
                                .ThenInclude(i => i.category)
                                .FirstOrDefault();
            }
        }

        public List<Product> GetTop5Products()
        {
            throw new NotImplementedException();
        }
    }
}
