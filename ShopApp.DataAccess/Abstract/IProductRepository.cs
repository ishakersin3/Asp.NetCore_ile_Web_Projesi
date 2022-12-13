using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApp.Entity;

namespace ShopApp.DataAccess.Abstract
{
    public interface IProductRepository:IRepository<Product>
    {
        Product GetProductDetails(int id);
        List<Product> GetPopulerProducts();
        List<Product> GetTop5Products();
    }
}
