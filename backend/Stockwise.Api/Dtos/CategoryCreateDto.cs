using System.ComponentModel.DataAnnotations;

namespace Stockwise.Api.Dtos;

public class CategoryCreateDto
{
  [Required]
  [MaxLength(150)]
  public required string Name { get; set; }
  
  [MaxLength(500)]
  public string? Description { get; set; }
  
}