using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Data;
using Stockwise.Api.Dtos;
using Stockwise.Api.Entities;
using Stockwise.Api.Exceptions;
using Stockwise.Api.Services;

namespace Stockwise.Tests.Services;

public class ProductServiceTests
{
    private StockwiseDbContext _context = null!;
    private ProductService _productService = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<StockwiseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new StockwiseDbContext(options);
        _productService = new ProductService(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            Quantity = 0,
            CategoryId = 1
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        
        var result = await _productService.GetByIdAsync(product.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(product.Id));
        Assert.That(result.Name, Is.EqualTo(product.Name));
        Assert.That(result.Sku, Is.EqualTo(product.Sku));
        Assert.That(result.Price, Is.EqualTo(product.Price));
    }
    
    [Test]
    public void GetByIdAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        Assert.ThrowsAsync<NotFoundException>(
            async () => await _productService.GetByIdAsync(1)
        );
    }

    [Test]
    public async Task getAll_WhenProductExists_ReturnsAllProducts()
    {
        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            Quantity = 0,
            CategoryId = 1
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        var result = await _productService.GetAllAsync();
        Assert.That(result.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetAllAsync_WhenNoProductsExist_ReturnsEmptyCollection()
    {
        var result = await _productService.GetAllAsync();
        
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CreateAsync_CreatesProduct()
    {
        var category = new Category()
        {
            Id = 1,
            Name = "Category 1",
            Description = "Category 1",
        };
        
        var product = new ProductCreateDto()
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            CategoryId = category.Id
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        var result = await _productService.CreateAsync(product);
        Assert.That(result.Name, Is.EqualTo(product.Name));
        Assert.That(result.Sku, Is.EqualTo(product.Sku));
    }
    
    [Test]
    public async Task CreateAsync_DuplicatedSku_ThrowsBusinessException()
    {
        var category = new Category()
        {
            Id = 1,
            Name = "Category 1",
            Description = "Category 1",
        };

        var product1 = new Product()
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            CategoryId = category.Id
        };
        _context.Products.Add(product1);
        
        var product = new ProductCreateDto()
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            CategoryId = category.Id
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        Assert.ThrowsAsync<BusinessException>(async () => await _productService.CreateAsync(product));
    }
    
    [Test]
    public async Task CreateAsync_CategoryDoesNotExist_ThrowsBusinessException()
    {
        var product = new ProductCreateDto()
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            CategoryId = 1
        };
        await _context.SaveChangesAsync();
        Assert.ThrowsAsync<BusinessException>(async () => await _productService.CreateAsync(product));
    }
    
    [Test]
    public async Task UpdateAsync_UpdatesProduct()
    {
        var category = new Category
        {
            Name = "Category 1",
            Description = "Category 1"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Description = "75% mechanical keyboard",
            Price = 299.90m,
            CategoryId = category.Id
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var dto = new ProductUpdateDto
        {
            Name = "Gaming Keyboard",
            Sku = "KB-002",
            Description = "Updated description",
            Price = 399.90m,
            CategoryId = category.Id
        };
        
        await _productService.UpdateAsync(product.Id, dto);

        var updatedProduct = await _context.Products.FindAsync(product.Id);

        Assert.That(updatedProduct, Is.Not.Null);
        Assert.That(updatedProduct.Name, Is.EqualTo(dto.Name));
        Assert.That(updatedProduct.Sku, Is.EqualTo(dto.Sku));
        Assert.That(updatedProduct.Description, Is.EqualTo(dto.Description));
        Assert.That(updatedProduct.Price, Is.EqualTo(dto.Price));
        Assert.That(updatedProduct.CategoryId, Is.EqualTo(dto.CategoryId));
    }
    
    [Test]
    public void UpdateAsync_ProductDoesNotExist_ThrowsNotFoundException()
    {
        var dto = new ProductUpdateDto
        {
            Name = "Gaming Keyboard",
            Sku = "KB-002",
            Description = "Updated description",
            Price = 399.90m,
            CategoryId = 1
        };

        Assert.ThrowsAsync<NotFoundException>(
            async () => await _productService.UpdateAsync(1, dto)
        );
    }
    
    [Test]
    public async Task UpdateAsync_DuplicatedSku_ThrowsBusinessException()
    {
        
        var category = new Category
        {
            Name = "Category 1",
            Description = "Category 1"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var product1 = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Price = 299.90m,
            CategoryId = category.Id
        };

        var product2 = new Product
        {
            Name = "Gaming Mouse",
            Sku = "MS-001",
            Price = 149.90m,
            CategoryId = category.Id
        };

        _context.Products.AddRange(product1, product2);
        await _context.SaveChangesAsync();

        var dto = new ProductUpdateDto
        {
            Name = "Gaming Mouse",
            Sku = "KB-001",
            Description = "Updated mouse",
            Price = 199.90m,
            CategoryId = category.Id
        };
        
        Assert.ThrowsAsync<BusinessException>(
            async () => await _productService.UpdateAsync(product2.Id, dto)
        );
    }
    
    [Test]
    public async Task UpdateAsync_CategoryDoesNotExist_ThrowsBusinessException()
    {
        var category = new Category
        {
            Name = "Category 1",
            Description = "Category 1"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Price = 299.90m,
            CategoryId = category.Id
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var dto = new ProductUpdateDto
        {
            Name = "Gaming Keyboard",
            Sku = "KB-002",
            Description = "Updated description",
            Price = 399.90m,
            CategoryId = 999
        };

        Assert.ThrowsAsync<BusinessException>(
            async () => await _productService.UpdateAsync(product.Id, dto)
        );
    }
    
    [Test]
    public async Task UpdateAsync_KeepsOwnSku_DoesNotThrowException()
    {
        var category = new Category
        {
            Name = "Category 1",
            Description = "Category 1"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Price = 299.90m,
            CategoryId = category.Id
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var dto = new ProductUpdateDto
        {
            Name = "Updated Keyboard",
            Sku = "KB-001",
            Price = 399.90m,
            CategoryId = category.Id
        };

        Assert.DoesNotThrowAsync(
            async () => await _productService.UpdateAsync(product.Id, dto)
        );
    }
    
    [Test]
    public async Task DeleteAsync_DeletesProduct()
    {
        var category = new Category
        {
            Name = "Category 1",
            Description = "Category 1"
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Mechanical Keyboard",
            Sku = "KB-001",
            Price = 299.90m,
            CategoryId = category.Id
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        await _productService.DeleteAsync(product.Id);

        var deletedProduct = await _context.Products.FindAsync(product.Id);

        Assert.That(deletedProduct, Is.Null);
    }
    
    [Test]
    public void DeleteAsync_ProductDoesNotExist_ThrowsNotFoundException()
    {
        Assert.ThrowsAsync<NotFoundException>(
            async () => await _productService.DeleteAsync(1)
        );
    }
    
}