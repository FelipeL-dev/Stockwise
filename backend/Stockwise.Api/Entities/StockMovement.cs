using Stockwise.Api.Enums;

namespace Stockwise.Api.Entities;

public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public StockMovementType Type { get; set; }

    public int OldQuantity { get; set; }
    
    public int Movement { get; set; }
    
    public int NewQuantity { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    
}