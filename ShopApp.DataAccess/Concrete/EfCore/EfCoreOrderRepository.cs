using Microsoft.EntityFrameworkCore;
using shopapp.entity;
using ShopApp.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public class EfCoreOrderRepository : EfCoreGenericRepository<Order, ShopContext>, IOrderRepository
    {
        public List<Order> GetOrders(string userId)
        {
            using (var context = new ShopContext())
            {
                var orders = context.Orders
                    .Include(i => i.OrderItems)
                    .ThenInclude(i => i.Product)
                    .AsQueryable();
                if (string.IsNullOrEmpty(userId))
                {
                    orders = orders.Where(İ => İ.UserId == userId);
                }
                return orders.ToList();
            }
        }
    }
}
