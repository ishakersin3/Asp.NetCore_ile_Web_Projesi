﻿using ShopApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.DataAccess.Abstract
{
    public interface ICategoryRepository:IRepository<Category>
    {       
        Category GetByIdWithProducts(int categoryId);
        void DeleteFromCategory(int ProductId, int CategoryId);
    }
}
