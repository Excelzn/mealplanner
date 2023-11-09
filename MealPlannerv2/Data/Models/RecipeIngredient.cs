namespace MealPlannerv2.Data.Models;

public class RecipeIngredient
{
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }
    public Guid RecipeId { get; set; }
    public double Amount { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public Guid UnitOfMeasureId { get; set; }
}