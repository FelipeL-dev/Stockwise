using Microsoft.EntityFrameworkCore;
using Stockwise.Api.Data;
using Stockwise.Api.Dtos;
using Stockwise.Api.Entities;
using Stockwise.Api.Enums;
using Stockwise.Api.Exceptions;
using Stockwise.Api.Services;

namespace Stockwise.Tests.Services;

public class StockMovementsTests
{
    
    private StockwiseDbContext _context = null!;
    private StockMovementService _movementService = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<StockwiseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new StockwiseDbContext(options);
        _movementService = new StockMovementService(_context);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task RegisterIncomingAsync_RegistersMovement()
    {

        Product product = new()
        {
            Name = "Test",
            Description = "Test",
            Quantity = 2,
            Price = 100,
            Sku = "TestSku",
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        StockMovementCreateDto stockMovement = new()
        {
            Reason = "",
            Quantity = 5,
        };
        
        var result = await _movementService.RegisterIncomingAsync(product.Id, stockMovement);
        
        
        Assert.That(result.Movement, Is.EqualTo(5));
        Assert.That(result.Type, Is.EqualTo(StockMovementType.Incoming));
        Assert.That(result.ProductId , Is.EqualTo(product.Id));
        Assert.That(product.Quantity, Is.EqualTo(result.OldQuantity + result.Movement));
        Assert.That(result.OldQuantity, Is.EqualTo(2));
        Assert.That(result.NewQuantity, Is.EqualTo(7));
        Assert.That(product.Quantity, Is.EqualTo(7));
        Assert.That(await _context.StockMovements.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task RegisterIncomingAsync_ProductNotFound_ThrowsNotFoundException()
    {
        StockMovementCreateDto stockMovement = new()
        {
            Reason = "",
            Quantity = 5,
        };
        Assert.ThrowsAsync<NotFoundException>(async () => await _movementService.RegisterIncomingAsync(1, stockMovement));
    }

    [Test]
    public async Task RegisterOutgoingAsync_RegistersMovement()
    {
        
        Product product = new()
        {
            Name = "Test",
            Description = "Test",
            Quantity = 5,
            Price = 100,
            Sku = "TestSku",
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        StockMovementCreateDto stockMovement = new()
        {
            Reason = "",
            Quantity = 2,
        };
        
        var result = await _movementService.RegisterOutgoingAsync(product.Id, stockMovement);
        
        
        Assert.That(result.Movement, Is.EqualTo(-2));
        Assert.That(result.Type, Is.EqualTo(StockMovementType.Outgoing));
        Assert.That(result.ProductId , Is.EqualTo(product.Id));
        Assert.That(product.Quantity, Is.EqualTo(result.OldQuantity + result.Movement));
        Assert.That(result.OldQuantity, Is.EqualTo(5));
        Assert.That(result.NewQuantity, Is.EqualTo(3));
        Assert.That(product.Quantity, Is.EqualTo(3));
        Assert.That(await _context.StockMovements.CountAsync(), Is.EqualTo(1));
    }
    
    [Test]
    public async Task RegisterOutgoingAsync_ProductNotFound_ThrowsNotFoundException()
    {
        StockMovementCreateDto stockMovement = new()
        {
            Reason = "",
            Quantity = 5,
        };
        Assert.ThrowsAsync<NotFoundException>(async () => await _movementService.RegisterOutgoingAsync(1, stockMovement));
    }
    
    [Test]
    public async Task RegisterOutgoingAsync_ProductQuantityLessThanMovementQuantity_ThrowsBusinessException()
    {
        
        Product product = new()
        {
            Name = "Test",
            Description = "Test",
            Quantity = 5,
            Price = 100,
            Sku = "TestSku",
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        StockMovementCreateDto stockMovement = new()
        {
            Reason = "",
            Quantity = 10,
        };
        
        Assert.ThrowsAsync<BusinessException>(async () => await _movementService.RegisterOutgoingAsync(product.Id, stockMovement));
    }
    
    [Test]
    public async Task RegisterAdjustmentAsync_RegistersMovement()
    {
        
        Product product = new()
        {
            Name = "Test",
            Description = "Test",
            Quantity = 5,
            Price = 100,
            Sku = "TestSku",
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        StockMovementCreateAdjustmentDto stockMovement = new()
        {
            Reason = "",
            Quantity = 2,
        };
        
        var result = await _movementService.RegisterAdjustmentAsync(product.Id, stockMovement);
        
        
        Assert.That(result.Movement, Is.EqualTo(-3));
        Assert.That(result.Type, Is.EqualTo(StockMovementType.Adjustment));
        Assert.That(result.ProductId , Is.EqualTo(product.Id));
        Assert.That(product.Quantity, Is.EqualTo(result.NewQuantity));
        Assert.That(result.OldQuantity, Is.EqualTo(5));
        Assert.That(result.NewQuantity, Is.EqualTo(2));
        Assert.That(product.Quantity, Is.EqualTo(2));
        Assert.That(await _context.StockMovements.CountAsync(), Is.EqualTo(1));
    }
    
    [Test]
    public async Task RegisterAdjustmentAsync_AdjustmentNotChangingQuantity_ThrowsBusinessException()
    {
        
        Product product = new()
        {
            Name = "Test",
            Description = "Test",
            Quantity = 5,
            Price = 100,
            Sku = "TestSku",
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        StockMovementCreateAdjustmentDto stockMovement = new()
        {
            Reason = "",
            Quantity = 5,
        };
        
        Assert.ThrowsAsync<BusinessException>(async () => await _movementService.RegisterAdjustmentAsync(product.Id, stockMovement));
    }
    
    [Test]
    public async Task RegisterAdjustmentAsync_ProductNotFound_ThrowsNotFoundException()
    {
        StockMovementCreateAdjustmentDto stockMovement = new()
        {
            Reason = "",
            Quantity = 5,
        };
        Assert.ThrowsAsync<NotFoundException>(async () => await _movementService.RegisterAdjustmentAsync(1, stockMovement));
    }
    
}