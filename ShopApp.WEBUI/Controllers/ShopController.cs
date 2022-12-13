using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WEBUI.Models;
using ShopApp.WEBUI.ViewModels;
using System.Linq;

namespace ShopApp.WEBUI.Controllers
{
    public class ShopController : Controller
    {
        private IProductService _productservice;

        public ShopController(IProductService productservice)
        {
            _productservice = productservice;
        }

        public IActionResult List()
        {
            var productviewmodel = new ProductListViewModels()
            {
                Products = _productservice.GetAll()
            };
            return View(productviewmodel);
        }
        public IActionResult Detail(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            Product product = _productservice.GetProductDetails((int)id);
            if (product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel
            {
                product= product,
                categories = product.ProductCategories.Select(i=>i.category).ToList()
            });
        }
    }
}
