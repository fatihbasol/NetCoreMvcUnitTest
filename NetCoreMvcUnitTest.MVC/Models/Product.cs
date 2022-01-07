using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace NetCoreMvcUnitTest.MVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }

    }
}
