namespace SALT.Domain;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IngredientType Type { get; set; }
}