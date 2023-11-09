using MealPlannerv2.Data.Models;

namespace MealPlannerv2.Data.Dtos;

public class RecipeDtoMinimal
{
    public RecipeDtoMinimal(Recipe recipe)
    {
        Id = recipe.Id.ToString();
        Name = recipe.Name;
    }
    public string Id { get; set; }
    public string Name { get; set; }
}