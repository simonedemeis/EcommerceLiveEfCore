using System.ComponentModel.DataAnnotations;

namespace EcommerceLiveEfCore.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        public double Price { get; set; }

        public string? Category { get; set; }
    }
}
