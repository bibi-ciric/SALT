using SALT.Domain;

namespace SALT.Repository;

public interface ICakeRepository
{
    Task<List<Ingredient>> GetAllIngredientsAsync();
    Task<List<CakeOriginal>> GetOriginalCakesAsync();
    Task<Ingredient?> GetIngredientByIdAsync(int id);
    Task AddOrderAsync(Order order);
    Task AddOrderCakesAsync(List<OrderCake> orderCakes);
    Task<bool> SaveChangesAsync();
}