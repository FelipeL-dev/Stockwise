using System.ComponentModel.DataAnnotations;

namespace Stockwise.Api.Dtos;

public class StockMovementCreateDto
{  
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [MaxLength(500)]
    public string? Reason { get; set; }
}