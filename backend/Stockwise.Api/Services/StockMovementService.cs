using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Data;
using Stockwise.Api.Dtos;
using Stockwise.Api.Entities;
using Stockwise.Api.Enums;
using Stockwise.Api.Exceptions;

namespace Stockwise.Api.Services;

public class StockMovementService : IStockMovementService
{
    private readonly StockwiseDbContext _context;
    
    public StockMovementService(StockwiseDbContext context)
    {
        _context = context;
    }

    private static StockMovementResponseDto MapToResponseDto(StockMovement stockMovement)
    {
        StockMovementResponseDto dto = new()
        {
            Id = stockMovement.Id,
            ProductId = stockMovement.ProductId,
            Quantity = stockMovement.Quantity,
            CreatedAt = stockMovement.CreatedAt,
            Reason = stockMovement.Reason,
            Type = stockMovement.Type
        };
        return dto;
    }

    public async Task<StockMovementResponseDto> RegisterIncomingAsync(int productId, StockMovementCreateDto dto)
    {
        Product? product =  await _context.Products.FindAsync(productId);
        
        if(product == null)
            throw new NotFoundException("Product not found");
        
        StockMovement stockMovement = new()
        {
            CreatedAt = DateTime.UtcNow,
            Reason = dto.Reason,
            Quantity = dto.Quantity,
            Type = StockMovementType.Incoming,
            ProductId = product.Id,
            Product = product
        };
        
        _context.StockMovements.Add(stockMovement);
        product.Quantity += stockMovement.Quantity;
        await _context.SaveChangesAsync();
        
        return MapToResponseDto(stockMovement);
    }

    public async Task<StockMovementResponseDto> RegisterOutgoingAsync(int productId, StockMovementCreateDto dto)
    {
        
        Product? product = await _context.Products.FindAsync(productId);
        if(product == null)
            throw new NotFoundException("Product not found");
        
        if(product.Quantity < dto.Quantity)
            throw new BusinessException("Insufficient stock");
        
        StockMovement stockMovement = new()
        {
            CreatedAt = DateTime.UtcNow,
            Quantity = dto.Quantity,
            Type = StockMovementType.Outgoing,
            Reason = dto.Reason,
            ProductId = product.Id,
            Product = product
        };
        
        _context.StockMovements.Add(stockMovement);
        
        product.Quantity -= stockMovement.Quantity;
        await _context.SaveChangesAsync();
        return MapToResponseDto(stockMovement);
    }

    public async Task<StockMovementResponseDto> RegisterAdjustmentAsync(int productId, StockMovementCreateAdjustmentDto dto)
    {
        Product? product = await _context.Products.FindAsync(productId);
        
        if(product == null)
            throw new NotFoundException("Product not found");
        
        StockMovement stockMovement = new()
        {
            CreatedAt = DateTime.UtcNow,
            Quantity = dto.Quantity,
            Type = StockMovementType.Adjustment,
            Reason =  dto.Reason,
            ProductId =  productId,
            Product = product
        };
        
        _context.StockMovements.Add(stockMovement);
        product.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();
        return MapToResponseDto(stockMovement);
    }

    public async Task<StockMovementResponseDto> GetByIdAsync(int id)
    {
        StockMovement? stockMovement = await _context.StockMovements.FindAsync(id);
        
        if(stockMovement == null)
            throw new NotFoundException("Stock Movement not found");
        
        return MapToResponseDto(stockMovement);
    }

    public async Task<IEnumerable<StockMovementResponseDto>> GetByProductIdAsync(int productId)
    {
        if (!await _context.Products.AnyAsync(p => p.Id == productId))
            throw new NotFoundException("Product not found");

        var stockMovements = await _context.StockMovements
            .Where(m => m.ProductId == productId)
            .Select(m => new StockMovementResponseDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                Quantity = m.Quantity,
                CreatedAt = m.CreatedAt,
                Reason = m.Reason,
                Type = m.Type
            })
            .ToListAsync();

        return stockMovements;
    }
}