namespace SALT.Domain; // <- file scoped namespace, from v10 and above

public class CakeOriginal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int BottomLayerId { get; set; }
    public Ingredient BottomLayer { get; set; }

    public int FillId { get; set; }
    public Ingredient Fill { get; set; }

    public int TopLayerId { get; set; }
    public Ingredient TopLayer { get; set; }
    
    public int ToppingId { get; set; }
    public Ingredient Topping { get; set; }
}
