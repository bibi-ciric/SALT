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

    // 1. Kreiranje krovne porudžbine
    var order = new Order
    {
        CustomerId = customerId,
        DateSubmitted = DateTime.Now,
        DateExpected = DateTime.Now.AddDays(2)
    };

    await _repository.AddOrderAsync(order);
    await _repository.SaveChangesAsync(); 

    var orderCakesList = new List<OrderCake>();

    //item and layer validation
    foreach (var item in items)
    {
        int finalOriginalId = item.CakeOriginalId ?? 1;
        int finalCreatedId = item.CakeCreatedId ?? 1;

        if (finalOriginalId != 1 && finalCreatedId != 1)
            throw new InvalidOperationException("Narudžbina ne može istovremeno sadržati i originalni i kastomizovani kolač.");
        
        if (finalOriginalId == 1 && finalCreatedId == 1)
            throw new InvalidOperationException("Morate odabrati ili originalni ili kastomizovani kolač za stavku.");

        //validation of the customized cake:
        if (finalCreatedId != 1)
        {
            var customCake = await _repository.GetCakeCreatedWithIngredientsAsync(finalCreatedId);
            
            if (customCake == null)
                throw new KeyNotFoundException($"Kastomizovani kolač sa ID-jem {finalCreatedId} ne postoji.");

            //check the bottom layer (either Bottom either None(1))
            if (customCake.BottomLayerId != 1 && customCake.BottomLayer?.Type != IngredientType.Bottom)
                throw new InvalidOperationException($"Kolač {finalCreatedId} ima neispravan donji sloj. Sastojak mora biti tipa 'Bottom'.");

            //check the fill layer (either Fill either None(1))
            if (customCake.FillId != 1 && customCake.Fill.Type != IngredientType.Fill)
                throw new InvalidOperationException($"Kolač {finalCreatedId} ima neispravan fil. Sastojak mora biti tipa 'Fill'.");

            //check the top layer (either Top either None(1))
            if (customCake.TopLayerId != 1 && customCake.TopLayer?.Type != IngredientType.Top)
                throw new InvalidOperationException($"Kolač {finalCreatedId} ima neispravan gornji sloj. Sastojak mora biti tipa 'Top'.");

            //check the topping (either Topping either None(1))
            if (customCake.ToppingId != 1 && customCake.Topping?.Type != IngredientType.Topping)
                throw new InvalidOperationException($"Kolač {finalCreatedId} ima neispravan preliv. Sastojak mora biti tipa 'Topping'.");
        }

        //after validation - create a part of an Order
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

public async Task<bool> CancelOrderAsync(int orderId)
{
    var order = await _repository.GetOrderByIdAsync(orderId);
    
    if (order == null)
        throw new KeyNotFoundException($"Porudžbina sa ID-jem {orderId} ne postoji.");

    if (order.Canceled)
        throw new InvalidOperationException("Porudžbina je već otkazana.");

    //canceling only possible if current date is at least 2 days before the date the cake will be finished
    if (DateTime.Now >= order.DateExpected.AddDays(-2))
    {
        throw new InvalidOperationException("Otkazivanje nije moguće. Prošao je rok za izmenu porudžbine (manje od 2 dana do isporuke).");
    }

    //so called ''soft delete'' - not possible to delete because of bussiness statistics
    order.Canceled = true;

    return await _repository.SaveChangesAsync();
}
}

public class OrderCakeDto
{
    public int? CakeOriginalId { get; set; }
    public int? CakeCreatedId { get; set; }
    public int Quantity { get; set; } = 1;
}