using MealPlannerv2.Data;
using MealPlannerv2.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerv2.Controllers;

[Route("api/v1/ingredients")]
public class IngredientController: ControllerBase
{
    private readonly MealPlannerDbContext _dbContext;

    public IngredientController(MealPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetIngredients()
    {
        var result = await _dbContext.Ingredients.ToListAsync();
        return Ok(result.Select(x => new IngredientDto
        {
            Id = x.Id.ToString(),
            Name = x.Name
        }));
    }
}