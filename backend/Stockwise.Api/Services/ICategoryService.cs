using Stockwise.Api.Dtos;

namespace Stockwise.Api.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
    
    Task<CategoryResponseDto> GetByIdAsync(int id);
    
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto categoryCreateDto);
    
    Task UpdateAsync(int id, CategoryUpdateDto categoryUpdateDto);
    
    Task DeleteAsync(int id);
}