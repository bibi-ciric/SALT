using SALT.Domain;
using SALT.Repository;

namespace SALT.Service;

public class OrderService
{
    private readonly ICakeRepository _repository;

    public OrderService(ICakeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Ingredient>> GetIngredientsAsync() => await _repository.GetAllIngredientsAsync();
    public async Task<List<CakeOriginal>> GetOriginalCakesAsync() => await _repository.GetOriginalCakesAsync();

    public async Task<bool> CreateOrderAsync(int customerId, List<OrderCakeDto> items)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("Porudžbina mora sadržati bar jedan kolač.");

        // 1. Kreiranje glavne porudžbine
        var order = new Order
        {
            CustomerId = customerId,
            DateSubmitted = DateTime.Now,
            DateExpected = DateTime.Now.AddDays(2) // Rok isporuke 2 dana
        };

        await _repository.AddOrderAsync(order);
        await _repository.SaveChangesAsync(); // Generiše Order.Id koji nam treba za stavke

        var orderCakesList = new List<OrderCake>();

        // 2. Prolazak kroz stavke i validacija biznis pravila
        foreach (var item in items)
        {
            int finalOriginalId = item.CakeOriginalId ?? 1;
            int finalCreatedId = item.CakeCreatedId ?? 1;

            // Biznis pravilo: Ne mogu oba biti naručena, niti oba izostavljena
            if (finalOriginalId != 1 && finalCreatedId != 1)
                throw new InvalidOperationException("Stavka ne može istovremeno sadržati i originalni i kastomizovani kolač.");
            
            if (finalOriginalId == 1 && finalCreatedId == 1)
                throw new InvalidOperationException("Morate odabrati ili originalni ili kastomizovani kolač za stavku.");

            var orderCake = new OrderCake
            {
                OrderId = order.Id,
                CakeOriginalId = finalOriginalId,
                CakeCreatedId = finalCreatedId,
                Quantity = item.Quantity
            };

            orderCakesList.Add(orderCake);
        }

        await _repository.AddOrderCakesAsync(orderCakesList);
        return await _repository.SaveChangesAsync();
    }
}

public class OrderCakeDto
{
    public int? CakeOriginalId { get; set; }
    public int? CakeCreatedId { get; set; }
    public int Quantity { get; set; } = 1;
}