using Stockwise.Api.Dtos;

namespace Stockwise.Api.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();

    Task<ProductResponseDto> GetByIdAsync(int id);
    
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
    
    Task  UpdateAsync(int id, ProductUpdateDto dto);
    
    Task DeleteAsync(int id);
}