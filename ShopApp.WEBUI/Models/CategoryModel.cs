using Microsoft.Build.Framework;
using ShopApp.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ShopApp.WEBUI.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage="Kategori Adı Zorunludur")]
        [StringLength(100,MinimumLength =5,ErrorMessage ="Kategori İçin 5 ile 100 Karakter Girmelisiniz")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Url Zorunludur")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Url İçin 5 ile 100 Karakter Girmelisiniz")]
        public string Url { get; set; }
        public List<Product> Products{ get; set; }

    }
}
