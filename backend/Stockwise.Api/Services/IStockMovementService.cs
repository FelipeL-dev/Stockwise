using Stockwise.Api.Dtos;

namespace Stockwise.Api.Services;

public interface IStockMovementService
{
    Task<StockMovementResponseDto> RegisterIncomingAsync(int productId, StockMovementCreateDto dto);
    
    Task<StockMovementResponseDto> RegisterOutgoingAsync(int productId, StockMovementCreateDto dto);
    
    Task<StockMovementResponseDto> RegisterAdjustmentAsync(int  productId, StockMovementCreateAdjustmentDto dto);
    
    Task<IEnumerable<StockMovementResponseDto>> GetByProductIdAsync(int productId);
    
    Task<StockMovementResponseDto> GetByIdAsync(int id);
}