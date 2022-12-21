using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WEBUI.Models;
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

        public IActionResult List(string category,int page=1)
        {
            const int pageSize = 2;
            var productviewmodel = new ProductListViewModels()
            {
                pageInfo = new PageInfo()
                {
                    TotalItems = _productservice.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productservice.GetProductsByCategory(category,page,pageSize)
            };
            return View(productviewmodel);
        }
        public IActionResult Detail(string url)
        {
            if (url == null)
            {
                return NotFound();
            }
            Product product = _productservice.GetProductDetails(url);
            if (product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel
            {
                Product = product,
                Categories = product.ProductCategories.Select(i=>i.Category).ToList()
            });
        }
        public IActionResult Search(string q) 
        {
            var productviewmodel = new ProductListViewModels()
            {              
                Products = _productservice.GetSearchResult(q)
            };
            return View(productviewmodel);
            
        }
    }
}
