using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WEBUI.Models;

namespace ShopApp.WEBUI.Controllers
{
    public class AdminController : Controller
    {
        private IProductService _productService; 

        public AdminController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult ProductList()
        {
            
            return View(new ProductListViewModels { Products = _productService.GetAll()});
        }
        public IActionResult CreateProduct()
        {
            return View();
            
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductModel product)
        {
            var entity = new Product()
            {
                Name= product.Name,
                Url= product.Url,
                Price= product.Price,
                ImageUrl= product.ImageUrl,
                Description= product.Description
            };
            _productService.Create(entity);
            return RedirectToAction("ProductList");

        }
        public IActionResult Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }          
            
            var product = _productService.GetById((int)id);
            if (product== null)
            {
                return NotFound();
            }
            var model = new ProductModel()
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Url = product.Url,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Description = product.Description
            };
            
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(ProductModel model)
        {
            var entity = _productService.GetById(model.ProductId);
            if (entity==null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            entity.Url = model.Url;
            entity.Price = model.Price;
            entity.ImageUrl = model.ImageUrl;
            entity.Description = model.Description;
            _productService.Update(entity);
            return RedirectToAction("ProductList");
        }
        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);
            if (entity == null)
            {
                return NotFound();
            }
            _productService.Delete(entity);
            return RedirectToAction("ProductList");
        }
    }
}
