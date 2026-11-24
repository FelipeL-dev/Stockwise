using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Data;
using Stockwise.Api.Dtos;
using Stockwise.Api.Entities;
using Stockwise.Api.Exceptions;

namespace Stockwise.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly StockwiseDbContext _context;
    
    public CategoryService(StockwiseDbContext context)
    {
        _context = context;
    }

    private static CategoryResponseDto MapToResponseDto(Category category)
    {
        CategoryResponseDto dto = new()
        {
            Id = category.Id,
            Name =  category.Name,
            Description = category.Description,
        };

        return dto;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _context.Categories.Select(category => MapToResponseDto(category)).ToListAsync();
        return categories;
    }

    public async Task<CategoryResponseDto> GetByIdAsync(int id)
    {
        var category = _context.Categories.Find(id);
        
        if(category == null)
            throw new NotFoundException("Category not found");
        
        return MapToResponseDto(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
    {
        Category category = new()
        {
            Name = dto.Name,
            Description = dto.Description,
        };
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return MapToResponseDto(category);
    }
    
    public async Task UpdateAsync(int id, CategoryUpdateDto dto)
    {
        Category? category = await _context.Categories.FindAsync(id);
        
        if (category == null)
            throw new NotFoundException("Category not found");
        
        category.Name = dto.Name;
        category.Description = dto.Description;
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        
        if (category == null)
            throw new NotFoundException("Category not found");
        if(category.Products.Count != 0)
            throw new BusinessException("You cannot delete categories with products in them");
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
    
}