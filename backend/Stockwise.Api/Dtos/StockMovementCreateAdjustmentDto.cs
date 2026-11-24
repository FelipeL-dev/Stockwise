using System.ComponentModel.DataAnnotations;

namespace Stockwise.Api.Dtos;

public class StockMovementCreateAdjustmentDto
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    
    [MaxLength(500)]
    public string? Reason { get; set; }
}