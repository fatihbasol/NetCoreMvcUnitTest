using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace NetCoreMvcUnitTest.MVC.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [MaxLength(50)]
        [Required]
        public string Color { get; set; }

    }
}
