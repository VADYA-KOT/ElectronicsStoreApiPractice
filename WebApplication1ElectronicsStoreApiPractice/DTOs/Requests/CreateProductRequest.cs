using System.ComponentModel.DataAnnotations;

namespace ElectronicsStoreApiPractice.DTOs.Requests
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "99999999.99",
            ParseLimitsInInvariantCulture = true)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public int? CategoryId { get; set; }

        public string? Description { get; set; }
    }
}
