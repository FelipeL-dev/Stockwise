using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Data;
using Stockwise.Api.Dtos;
using Stockwise.Api.Entities;
using Stockwise.Api.Exceptions;

namespace Stockwise.Api.Services;

public class ProductService : IProductService
{
    private readonly StockwiseDbContext _context;
    
    public ProductService(StockwiseDbContext context)
    {
        _context = context;
    }

    private static ProductResponseDto MapToResponseDto(Product product)
    {
        ProductResponseDto dto = new()
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            Quantity = product.Quantity,
        };
        return dto;
    }
    
    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var productsDto = await _context.Products
            .Select(product => MapToResponseDto(product))
            .ToListAsync();

        return productsDto;
    }

    public async Task<ProductResponseDto> GetByIdAsync(int id)
    {
        Product? product = await _context.Products.FindAsync(id);
        
        if (product is null)
           throw new NotFoundException("Product not found");

        ProductResponseDto productResponseDto = MapToResponseDto(product);
        return productResponseDto;
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        Product product = new()
        {
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            Sku = dto.Sku,
            Description = dto.Description,
            Price = dto.Price,
        };
        
        if(await _context.Products.AnyAsync(x => x.Sku == dto.Sku))
            throw new BusinessException($"Product with Sku {dto.Sku} already exists");
        
        if(!await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
            throw new BusinessException($"Category with id {dto.CategoryId} does not exist");

        
        _context.Products.Add(product);
        
        
        await _context.SaveChangesAsync();

        return MapToResponseDto(product);
    }

    public async Task UpdateAsync(int id, ProductUpdateDto dto)
    {
        Product? product = await _context.Products.FindAsync(id);
        
        if(product is null)
            throw new NotFoundException("Product not found");
        
        if(await _context.Products.AnyAsync(x => x.Sku == dto.Sku && x.Id != product.Id))
            throw new BusinessException($"Product with Sku {dto.Sku} already exists");
        
        if(!await _context.Categories.AnyAsync(x => x.Id == dto.CategoryId))
            throw new BusinessException($"Category with id {dto.CategoryId} does not exist");

        product.CategoryId = dto.CategoryId;
        product.Description = dto.Description;
        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Sku = dto.Sku;
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Product? product = await _context.Products.FindAsync(id);

        if (product is null)
            throw new NotFoundException($"Product with ID {id} was not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
    
    
    
}