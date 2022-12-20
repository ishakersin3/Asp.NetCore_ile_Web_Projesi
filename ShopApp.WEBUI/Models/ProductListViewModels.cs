using ShopApp.Entity;
using System;
using System.Collections.Generic;

namespace ShopApp.WEBUI.Models
{
    public class PageInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string CurrentCategory { get; set; }

        public int TotalPage()
        {
            return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        }
    }
    public class ProductListViewModels
    {
        public PageInfo pageInfo { get; set; }
        public List<Product> Products { get; set; }
    }
}
