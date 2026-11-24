using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Data;
using Stockwise.Api.Dtos;
using Stockwise.Api.Entities;
using Stockwise.Api.Exceptions;
using Stockwise.Api.Services;

namespace Stockwise.Tests.Services;

public class CategoryServiceTests
{
    private StockwiseDbContext _context = null!;
    private CategoryService _categoryService = null!;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<StockwiseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new StockwiseDbContext(options);
        _categoryService = new CategoryService(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task GetAllCategoriesAsync_WhenCategoryExists_ReturnsAllCategories()
    {
        var category = new Category()
        {
            Name = "TestCategory",
            Description = "Test category description",
        };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        var result = await _categoryService.GetAllAsync();
        Assert.That(result.Count, Is.GreaterThan(0));
        Assert.That(result.First().Name, Is.EqualTo(category.Name));
    }

    [Test]
    public async Task GetAllCategoriesAsync_WhenCategoryDoesNotExist_ReturnsEmptyList()
    {
        var result = await _categoryService.GetAllAsync();
        
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCategoryByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        Category category = new Category()
        {
            Name = "TestCategory",
            Description = "Test category description",
        };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        
        var result = await _categoryService.GetByIdAsync(category.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(category.Id));
        Assert.That(result.Name, Is.EqualTo(category.Name));
        Assert.That(result.Description, Is.EqualTo(category.Description));
    }

    [Test]
    public void GetCategoryByIdAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        Assert.ThrowsAsync<NotFoundException>(async () => await _categoryService.GetByIdAsync(1));
    }

    [Test]
    public async Task CreateCategoryAsync_CreatesCategory()
    {
        var category = new CategoryCreateDto()
        {
            Name = "TestCategory",
            Description = "Test category description",
        };
        
        var result = await _categoryService.CreateAsync(category);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(category.Name));
        Assert.That(result.Description, Is.EqualTo(category.Description));
    }

    [Test]
    public async Task UpdateCategoryAsync_UpdatesCategory()
    {
        var category = new Category()
        {
            Name = "TestCategory",
            Description = "Test category description",
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var dto = new CategoryUpdateDto()
        {
            Name = "TestCategory1",
            Description = "Test category description2",
        };
        
        await _categoryService.UpdateAsync(category.Id, dto);
        
        var updatedCategory = await _categoryService.GetByIdAsync(category.Id);
        
        Assert.That(updatedCategory.Name, Is.EqualTo(dto.Name));
        Assert.That(updatedCategory.Description, Is.EqualTo(dto.Description));
    }

    [Test]
    public void UpdateCategoryAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        var dto = new CategoryUpdateDto()
        {
            Name = "TestCategory",
            Description = "Test category description",
        };
        Assert.ThrowsAsync<NotFoundException>(async () => await _categoryService.UpdateAsync(1,dto));
    }

    [Test]
    public async Task DeleteCategoryAsync_DeletesCategory()
    {
        Category category = new()
        {  
            Id = 1,
            Name = "TestCategory",
            Description = "Test category description",
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        await _categoryService.DeleteAsync(category.Id);
        
        var deletedCategory = await _context.Categories.FindAsync(category.Id);
        
        Assert.That(deletedCategory, Is.Null);
    }

    [Test]
    public void DeleteCategoryAsync_WhenCategoryDoesNotExist_ThrowsNotFoundException()
    {
        Assert.ThrowsAsync<NotFoundException>(() => _categoryService.DeleteAsync(1));
    }

    [Test]
    public async Task DeleteCategoryAsync_WhenProductsOnCategory_ThrowsBusinessException()
    {
        Product product = new()
        {
            Name = "TestProduct",
            Description = "Test category description",
            Sku =  "TestSku",
        };
        Category category = new()
        {
            Id = 1,
            Name = "TestCategory",
            Description = "Test category description",
            Products = [product]
        };
        product.CategoryId = category.Id;
        product.Category = category;
        _context.Categories.Add(category);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        Assert.ThrowsAsync<BusinessException>(async () => await _categoryService.DeleteAsync(category.Id));
    }
}