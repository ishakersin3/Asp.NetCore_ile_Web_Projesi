using shopapp.entity;
using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private IOrderRepository _orderrepository;
        public OrderManager(IOrderRepository orderrepository)
        {
            this._orderrepository = orderrepository;
        }
        public void Create(Order entity)
        {
            _orderrepository.Create(entity);
        }

        public List<Order> GetOrders(string userId)
        {
            return _orderrepository.GetOrders(userId);
        }
    }
}
