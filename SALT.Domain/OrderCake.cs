namespace SALT.Domain;

public class OrderCake
{
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int CakeOriginalId { get; set; }
    public CakeOriginal CakeOriginal { get; set; }
    public int CakeCreatedId { get; set; }
    public CakeCreated CakeCreated { get; set; }

    public int Quantity { get; set; }

}