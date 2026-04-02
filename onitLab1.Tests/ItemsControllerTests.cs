using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnitLab1.Controllers;
using OnitLab1.Data;
using OnitLab1.Models;
using Xunit;

namespace onitLab1.Tests;

public class ItemsControllerTests
{
    [Fact]
    public async Task GetItems_ReturnsAllItems()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        using var context = new AppDbContext(options);
        
        context.Items.Add(new Item 
        { 
            Name = "Test Item", 
            Description = "Description",
            CreatedAt = DateTime.UtcNow 
        });
        await context.SaveChangesAsync();
        
        var controller = new ItemsController(context);
        var result = await controller.GetItems();
        
        var items = result.Value;
        Assert.NotNull(items);
        Assert.Single(items);
    }
}