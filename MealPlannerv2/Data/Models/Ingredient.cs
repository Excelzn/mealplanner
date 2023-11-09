using System.Globalization;
using MealPlannerv2.Utilities;

namespace MealPlannerv2.Data.Models;

public class Ingredient
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public IEnumerable<RecipeIngredient> RecipeIngredients { get; set; }
}