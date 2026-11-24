using System.ComponentModel.DataAnnotations;

namespace Stockwise.Api.Dtos;

public class ProductCreateDto
{
    [Required]
    [MaxLength(150)]
    public required string Name { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Sku { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Range(typeof(decimal), "0.01", "9999999.99",
        ParseLimitsInInvariantCulture = true)]
    public decimal Price { get; set; }
    
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}