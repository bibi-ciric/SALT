using Microsoft.EntityFrameworkCore;
using SALT.Domain;

namespace SALT.Repository;

public class CakeRepository : ICakeRepository
{
    private readonly DataContext _context;

    public CakeRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Ingredient>> GetAllIngredientsAsync()
    {
        return await _context.Ingredients.ToListAsync();
    }

    public async Task<List<CakeOriginal>> GetOriginalCakesAsync()
    {
        return await _context.CakeOriginals.ToListAsync();
    }

    public async Task<Ingredient?> GetIngredientByIdAsync(int id)
    {
        return await _context.Ingredients.FindAsync(id);
    }

    public async Task AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task AddOrderCakesAsync(List<OrderCake> orderCakes)
    {
        await _context.OrderCakes.AddRangeAsync(orderCakes);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync()) > 0;
    }
}