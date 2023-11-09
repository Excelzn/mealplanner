namespace MealPlannerv2.Data.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Instructions { get; set; }
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
}