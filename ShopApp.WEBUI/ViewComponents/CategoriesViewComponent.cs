using Microsoft.AspNetCore.Mvc;
using ShopApp.Business.Abstract;
using System.Collections.Generic;

namespace ShopApp.WEBUI.ViewComponents
{
    public class CategoriesViewComponent:ViewComponent
    {
        private ICategoryService _categoryService;
        public CategoriesViewComponent(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }
        public IViewComponentResult Invoke()
        {
            if (RouteData.Values["action"].ToString() == "list")
            {
                ViewBag.SelectedCategory = RouteData?.Values["id"];
            }
            return View(_categoryService.GetAll());            
        }

    }
}
