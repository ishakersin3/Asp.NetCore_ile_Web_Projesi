using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopApp.Business.Abstract;
using ShopApp.Entity;
using ShopApp.WEBUI.Models;
using System.Linq;

namespace ShopApp.WEBUI.Controllers
{
    public class AdminController : Controller
    {
        private void CreateMessage(string message,string alerttype)
        {
            var msg = new AlertMessage()
            {
                Message = message,
                AlertType = alerttype
            };
            TempData["Message"] = JsonConvert.SerializeObject(msg);

        }

        private IProductService _productService;

        private ICategoryService _categoryService;
        public AdminController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public IActionResult ProductList()
        {
            return View(new ProductListViewModels { Products = _productService.GetAll() });
        }       
        public IActionResult ProductCreate()
        {
            return View();

        }
        [HttpPost]
        public IActionResult ProductCreate(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name = product.Name,
                    Url = product.Url,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Description = product.Description
                };
                if (_productService.Create(entity))
                {
                    CreateMessage("kayıt eklendi","success");                   
                    return RedirectToAction("ProductList");
                }
                CreateMessage(_productService.ErrorMessage, "danger");
                return View(product);
            }
            return View(product);

        }
        public IActionResult ProductEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetByIdWithCategories((int)id);
            if (product == null)
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
                Description = product.Description,
                IsApproved= product.IsApproved,
                IsHome= product.IsHome,
                SelectedCategories = product.ProductCategories.Select(x=>x.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();

            return View(model);
        }
        [HttpPost]
        public IActionResult ProductEdit(ProductModel model,int[] CategoryIds)
        {
            if (ModelState.IsValid)
            {
                var entity = _productService.GetById(model.ProductId);
                if (entity == null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Url = model.Url;
                entity.Price = model.Price;
                entity.ImageUrl = model.ImageUrl;
                entity.Description = model.Description;  
                entity.IsHome= model.IsHome;
                entity.IsApproved= model.IsApproved;
                
                if (_productService.Update(entity, CategoryIds))
                {
                    CreateMessage("Kayıt Güncellendi", "success");
                    return RedirectToAction("ProductList");
                }
                CreateMessage(_productService.ErrorMessage, "danger");               
            }
            ViewBag.Categories = _categoryService.GetAll();
            return View(model);
                        
            
        }
        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);
            if (entity == null)
            {
                return NotFound();
            }
            _productService.Delete(entity);

            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} İsimli Ürün Silindi.",
                AlertType = "danger"
            };
            TempData["Message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("ProductList");
           
        }
        public IActionResult CategoryList()
        {

            return View(new CategoryListViewModels { Categories = _categoryService.GetAll() });
        }
        public IActionResult CategoryCreate()
        {
            return View();

        }
        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
            var entity = new Category()
            {
                Name = category.Name,
                Url = category.Url,
               
            };
            _categoryService.Create(entity);
            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} İsimli Kategori Eklendi.",
                AlertType = "success"
            };
            TempData["Message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("CategoryList");
            }
            return View(category);

        }
        public IActionResult CategoryEdit(int? id)
        {           
                if (id == null)
                {
                    return NotFound();
                }

                var category = _categoryService.GetByIdWithProducts((int)id);
                if (category == null)
                {
                    return NotFound();
                }
                var model = new CategoryModel()
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Url = category.Url,
                    Products = category.ProductCategories.Select(x => x.Product).ToList()
                };

                return View(model);
            
            
        }
        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _categoryService.GetById(model.CategoryId);
                if (entity == null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Url = model.Url;
                _categoryService.Update(entity);

                var msg = new AlertMessage()
                {
                    Message = $"{model.Name} İsimli Kategori Güncellendi.",
                    AlertType = "success"
                };
                TempData["Message"] = JsonConvert.SerializeObject(msg);
                return RedirectToAction("CategoryList");
            }
            return View(model);

        }
        public IActionResult DeleteCategory(int categoryId)
        {
            var entity = _categoryService.GetById(categoryId);
            if (entity == null)
            {
                return NotFound();
            }
            _categoryService.Delete(entity);

            var msg = new AlertMessage()
            {
                Message = $"{entity.Name} İsimli Kategori Silindi.",
                AlertType = "danger"
            };
            TempData["Message"] = JsonConvert.SerializeObject(msg);
            return RedirectToAction("CategoryList");
        }
        [HttpPost]
        public IActionResult DeleteFromCategory(int ProductId,int CategoryId)
        {
            _categoryService.DeleteFromCategory(ProductId,CategoryId);
            return Redirect("/admin/categories/"+CategoryId);
        }
    }
}
