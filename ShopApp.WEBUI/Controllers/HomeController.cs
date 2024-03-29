﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.WEBUI.Models;
using System;
using System.Collections.Generic;

namespace ShopApp.WEBUI.Controllers
{
    // localhost:5000/home

    public class HomeController:Controller
    {
        private IProductService _productservice;

        public HomeController(IProductService productservice)
        {
            this._productservice = productservice;
        }

        public IActionResult Index()
        {

            var productViewModel = new ProductListViewModels()
            {
                Products = _productservice.GetHomePageProducts()
            };
            return View(productViewModel);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View("MyView");
        }
    }
}
