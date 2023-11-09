using MealPlannerv2.Data.Models;

namespace MealPlannerv2.Data.Dtos;

public class RecipeIngredientDto
{
    public RecipeIngredientDto()
    {
        
    }

    public RecipeIngredientDto(RecipeIngredient ingredient)
    {
        Name = ingredient.Ingredient.Name;
        Amount = ingredient.Amount;
        UnitOfMeasure = ingredient.UnitOfMeasure.Name;
    }
    public string Name { get; set; }
    public double Amount { get; set; }
    public string UnitOfMeasure { get; set; }
}