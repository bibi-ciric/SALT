namespace SALT.Domain;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}