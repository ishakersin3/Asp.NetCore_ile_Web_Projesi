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
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ShopContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int ProductId, int CategoryId)
        {
            using(var context = new ShopContext())
            {
                var cmd = "delete from ProductCategory Where ProductId=@p0 and CategoryId=@p1";
                context.Database.ExecuteSqlRaw(cmd,ProductId,CategoryId);
            }
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            using(var context = new ShopContext())
            {
                return context.Categories
                    .Where(x => x.CategoryId == categoryId)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(t => t.Product)
                    .FirstOrDefault();
            }

        }
        
    }
}
