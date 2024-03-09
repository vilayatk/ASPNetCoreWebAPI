using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFirstAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        [Required] // Must be provided // Bad Request 400 if absent via [ApiController]
        public int CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category? Category { get; set; }

    }
}
