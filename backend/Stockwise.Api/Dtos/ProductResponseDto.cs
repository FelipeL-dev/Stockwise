using Stockwise.Api.Entities;

namespace Stockwise.Api.Dtos;

public class ProductResponseDto
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public required string Sku { get; set; }
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
    
    public int CategoryId { get; set; }
    
}