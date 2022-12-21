using Microsoft.Build.Framework;
using ShopApp.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ShopApp.WEBUI.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }

        //[Required(ErrorMessage = "Name Zorunlu Bir Alandır")]
        //[StringLength(60,MinimumLength =5,ErrorMessage ="Ürün İsmi 5 İle 60 Karakter Arasında Olmalıdır")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Url Zorunlu Bir Alandır")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Price Zorunlu Bir Alandır")]
        [Range(1,20000,ErrorMessage = "Price İçin 1 İle 20000 Arasında Olmalıdır")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "Description Zorunlu Bir Alandır")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Açıklama Kısmı 5 İle 100 Karakter Arasında Olmalıdır")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ImageUrl Zorunlu Bir Alandır")]
        public string ImageUrl { get; set; }

        public bool IsApproved { get; set; }
        public bool IsHome { get; set; }
        public List<Category> SelectedCategories { get; set; }       
    }
}
