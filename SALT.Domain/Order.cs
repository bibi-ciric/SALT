namespace SALT.Domain;

public class Order
{
    public int Id { get; set; }
    
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public DateTime DateSubmitted { get; set; } = DateTime.Now;
    public DateTime DateExpected { get; set; }
    public DateTime? DateShipped { get; set; }
    public bool Canceled { get; set; } = false;
    public ICollection<OrderCake> OrderCakes { get; set; } = new List<OrderCake>();
}