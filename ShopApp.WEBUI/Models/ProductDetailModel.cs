using ShopApp.Entity;
using System.Collections.Generic;

namespace ShopApp.WEBUI.Models
{
    public class ProductDetailModel
    {
        public Product product { get; set; }
        public List<Category> categories { get; set; }
    }
}
