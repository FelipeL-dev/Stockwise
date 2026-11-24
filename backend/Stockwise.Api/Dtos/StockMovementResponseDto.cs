using Stockwise.Api.Enums;

namespace Stockwise.Api.Dtos;

public class StockMovementResponseDto
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    
    public string? Reason { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public StockMovementType Type { get; set; }
    
    public int Quantity { get; set; }
    
}