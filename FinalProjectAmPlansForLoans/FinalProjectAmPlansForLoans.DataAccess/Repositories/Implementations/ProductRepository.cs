using FinalProjectAmPlansForLoans.DataAccess.DataContext;
using FinalProjectAmPlansForLoans.DataAccess.Repositories;
using FinalProjectAmPlansForLoans.Domain.Models;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly AmPlansDbContext _context;

    public ProductRepository(AmPlansDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
    // Method to get admin fee by product ID asynchronously
    public decimal GetAdminFeeByProductId(int productId)
    {
        var product = _context.Products.Find(productId);
        if (product == null)
        {
            throw new ArgumentException("Invalid product ID.");
        }

        return (decimal)product.AdminFee;
    }




}

