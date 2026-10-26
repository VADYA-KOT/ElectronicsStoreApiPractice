using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ElectronicsStoreApiPractice.DTOs.Requests;
using ElectronicsStoreApiPractice.DTOs.Response;
using ElectronicsStoreApiPractice.Data;

namespace ElectronicsStoreApiPractice.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly StoreDbContext _context;

    public CategoriesController(StoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count()
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count()
            })
            .FirstOrDefaultAsync();

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponse>> Create(
        [FromBody] CreateCategoryRequest request)
    {
        string name = request.Name.Trim();

        if (name.Length < 2 || name.Length > 100)
        {
            return BadRequest(new
            {
                message = "Название должно содержать от 2 до 100 символов."
            });
        }

        bool idExists = await _context.Categories
            .AnyAsync(c => c.Id == request.Id);

        if (idExists)
        {
            return Conflict(new
            {
                message = $"Категория с ID {request.Id} уже существует."
            });
        }

        bool nameExists = await _context.Categories
            .AnyAsync(c => c.Name == name);

        if (nameExists)
        {
            return Conflict(new
            {
                message = "Категория с таким названием уже существует."
            });
        }

        var category = new Category
        {
            Id = request.Id,
            Name = name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            ProductCount = 0
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            response);
    }
}
