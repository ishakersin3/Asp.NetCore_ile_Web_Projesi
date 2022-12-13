﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.Entity
{
    public class ProductCategory
    {
        public int CategoryId { get; set; } 
        public Category category { get; set; }
        public int ProductId { get; set; }
        public Product product { get; set; }
    }
}
