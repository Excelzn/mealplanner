using MealPlannerv2.Data.Models;

namespace MealPlannerv2.Data.Dtos;

public class RecipeDto
{
    public RecipeDto()
    {
        
    }

    public RecipeDto(Recipe recipe)
    {
        Name = recipe.Name;
        Id = recipe.Id.ToString();
        Instructions = recipe.Instructions;
        Ingredients = recipe.RecipeIngredients.Select(x => new RecipeIngredientDto(x));
    }
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<RecipeIngredientDto> Ingredients { get; set; }
    public string Instructions { get; set; }
}