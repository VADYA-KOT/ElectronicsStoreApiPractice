using ElectronicsStoreApiPractice.Data;
using ElectronicsStoreApiPractice.DTOs.Requests;
using ElectronicsStoreApiPractice.DTOs.Response;
using ElectronicsStoreApiPractice.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStoreApiPractice.Services;

public enum ProductServiceStatus
{
    Success,
    ProductNotFound,
    CategoryNotFound
}

public sealed record ProductServiceResult(
    ProductServiceStatus Status,
    ProductResponse? Product = null);

public class ProductService
{
    private readonly StoreDbContext _context;

    public ProductService(StoreDbContext context)
    {
        _context = context;
    }

    // Получение списка товаров с фильтрацией и пагинацией.
    public async Task<ProductListResponse> GetAllAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int? maxQuantity,
        string sortBy,
        bool descending,
        bool inStockOnly,
        int page,
        int pageSize)
    {
        IQueryable<Product> query =
            _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();

            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{term}%"));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price <= maxPrice.Value);
        }

        if (maxQuantity.HasValue)
        {
            query = query.Where(p =>
                p.Quantity <= maxQuantity.Value);
        }

        if (inStockOnly)
        {
            query = query.Where(p => p.Quantity > 0);
        }

        int totalCount = await query.CountAsync();

        query = (sortBy, descending) switch
        {
            ("id", false) =>
                query.OrderBy(p => p.Id),

            ("id", true) =>
                query.OrderByDescending(p => p.Id),

            ("name", false) =>
                query.OrderBy(p => p.Name).ThenBy(p => p.Id),

            ("name", true) =>
                query.OrderByDescending(p => p.Name).ThenBy(p => p.Id),

            ("price", false) =>
                query.OrderBy(p => p.Price).ThenBy(p => p.Id),

            ("price", true) =>
                query.OrderByDescending(p => p.Price).ThenBy(p => p.Id),

            ("quantity", false) =>
                query.OrderBy(p => p.Quantity).ThenBy(p => p.Id),

            ("quantity", true) =>
                query.OrderByDescending(p => p.Quantity).ThenBy(p => p.Id),

            _ => query.OrderBy(p => p.Id)
        };

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null
                    ? p.Category.Name
                    : null,
                Description = p.Description
            })
            .ToListAsync();

        return new ProductListResponse
        {
            Items = products,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }

    // Получение одного товара.
    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null
                    ? p.Category.Name
                    : null,
                Description = p.Description
            })
            .FirstOrDefaultAsync();
    }

    // Получение товаров определённой категории.
    public async Task<List<ProductResponse>> GetByCategoryAsync(
        int categoryId)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Id)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null
                    ? p.Category.Name
                    : null,
                Description = p.Description
            })
            .ToListAsync();
    }

    // Создание товара.
    public async Task<ProductServiceResult> CreateAsync(
        CreateProductRequest request)
    {
        string? categoryName = null;

        if (request.CategoryId.HasValue)
        {
            categoryName = await _context.Categories
                .Where(c => c.Id == request.CategoryId.Value)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            if (categoryName is null)
            {
                return new ProductServiceResult(
                    ProductServiceStatus.CategoryNotFound);
            }
        }

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId,
            Description = request.Description
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            Description = product.Description
        };

        return new ProductServiceResult(
            ProductServiceStatus.Success,
            response);
    }

    // Обновление товара.
    public async Task<ProductServiceStatus> UpdateAsync(
        int id,
        UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return ProductServiceStatus.ProductNotFound;
        }

        if (request.CategoryId.HasValue)
        {
            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId.Value);

            if (!categoryExists)
            {
                return ProductServiceStatus.CategoryNotFound;
            }
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.CategoryId = request.CategoryId;
        product.Description = request.Description;

        await _context.SaveChangesAsync();

        return ProductServiceStatus.Success;
    }

    // Удаление товара.
    public async Task<ProductServiceStatus> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return ProductServiceStatus.ProductNotFound;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return ProductServiceStatus.Success;
    }
}
