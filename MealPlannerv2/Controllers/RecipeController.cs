using MealPlannerv2.Data;
using MealPlannerv2.Data.Dtos;
using MealPlannerv2.Data.Models;
using MealPlannerv2.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerv2.Controllers;

[Route("/api/v1/recipes")]
public class RecipeController: ControllerBase
{
    private readonly MealPlannerDbContext _dbContext;

    public RecipeController(MealPlannerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeDto dto, CancellationToken cancellationToken)
    {
        var recipe = new Recipe
        {
            Name = dto.Name,
            Instructions = dto.Instructions,
            RecipeIngredients = new List<RecipeIngredient>()
        };
        //Resolve ingredients

        recipe.RecipeIngredients = await ProcessIngredients(dto.Ingredients, cancellationToken);

        await _dbContext.Recipes.AddAsync(recipe, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetRecipes()
    {
        var result = await _dbContext.Recipes.ToListAsync();
        return Ok(result.Select(x => new RecipeDtoMinimal(x)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(string id)
    {
        var result = await _dbContext.Recipes
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.UnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id.ToString() == id);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(new RecipeDto(result));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecipe(string id, [FromBody] RecipeDto recipeDto, CancellationToken cancellationToken)
    {
        var recipe = await _dbContext.Recipes
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.Ingredient)
            .Include(x => x.RecipeIngredients)
            .ThenInclude(x => x.UnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id.ToString() == id, cancellationToken);
        if (recipe == null)
        {
            return NotFound();
        }

        if (id != recipeDto.Id)
        {
            return BadRequest("ID does not match resource.");
        }

        recipe.Name = recipeDto.Name;
        recipe.Instructions = recipeDto.Instructions;
        
        //remove ingredients that have been deleted.
        var removed = recipe.RecipeIngredients.Where(x =>
            recipeDto.Ingredients.All(y => y.Name.ToNormal() != x.Ingredient.NormalizedName)).ToList();
        foreach (var recipeIngredient in removed)
        {
            recipe.RecipeIngredients.Remove(recipeIngredient);
        }

        var added = recipeDto.Ingredients.Where(x =>
            recipe.RecipeIngredients.All(y => y.Ingredient.NormalizedName != x.Name.ToNormal())).ToList();
        var recipeIngredients = await ProcessIngredients(added, cancellationToken);
        foreach (var recipeIngredient in recipeIngredients)
        {
            recipe.RecipeIngredients.Add(recipeIngredient);
        }

        var unitsOfMeasure = await _dbContext.UnitOfMeasure.ToListAsync(cancellationToken);
        var remaining =
            recipe.RecipeIngredients.Where(x => added.All(y => y.Name.ToNormal() != x.Ingredient.NormalizedName));
        foreach (var ingredient in remaining)
        {
            var dto = recipeDto.Ingredients.First(x => x.Name.ToNormal() == ingredient.Ingredient.NormalizedName);
            var uom = unitsOfMeasure.FirstOrDefault(x =>
                x.Name.ToNormal().Equals(dto.UnitOfMeasure.ToNormal())) ?? new UnitOfMeasure
            {
                Name = dto.UnitOfMeasure
            };
            ingredient.UnitOfMeasure = uom;
            ingredient.Amount = dto.Amount;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveRecipe(string id)
    {
        var item = await _dbContext.Recipes.FirstOrDefaultAsync(x => x.Id.ToString() == id);
        if (item != null)
        {
            _dbContext.Recipes.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        return NoContent();
    }


    private async Task<List<RecipeIngredient>> ProcessIngredients(IEnumerable<RecipeIngredientDto> incomingIngredients, CancellationToken cancellationToken)
    {
        var ingredientDtos = incomingIngredients.ToList();
        var normalizedNames = ingredientDtos.Select(x => x.Name.ToNormal());
        var ingredients =
            await _dbContext.Ingredients.Where(x => normalizedNames.Any(y => y == x.NormalizedName)).ToListAsync(cancellationToken);
        var unitsOfMeasure = await _dbContext.UnitOfMeasure.ToListAsync(cancellationToken);
        //Process existing ingredients
        var result = new List<RecipeIngredient>();
        foreach (var ingredient in ingredients)
        {
            var ingredientDto = ingredientDtos.First(x => x.Name.ToNormal() == ingredient.NormalizedName);
            var uom = unitsOfMeasure.FirstOrDefault(x =>
                x.Name.ToNormal().Equals(ingredientDto.UnitOfMeasure.ToNormal())) ?? new UnitOfMeasure
            {
                Name = ingredientDto.UnitOfMeasure
            };

            var recipeIngredient = new RecipeIngredient
            {
                Ingredient = ingredient,
                Amount = ingredientDto.Amount,
                UnitOfMeasure = uom,
            };
            result.Add(recipeIngredient);
        }
        //process new ingredients
        var newIngredients =
            ingredientDtos.Where(x => ingredients.All(y => y.NormalizedName != x.Name.ToNormal()));
        foreach (var ingredientDto in newIngredients)
        {
            var uom = unitsOfMeasure.FirstOrDefault(x =>
                x.Name.ToNormal().Equals(ingredientDto.UnitOfMeasure.ToNormal())) ?? new UnitOfMeasure
            {
                Name = ingredientDto.UnitOfMeasure
            };

            var recipeIngredient = new RecipeIngredient
            {
                Ingredient = new Ingredient
                {
                    Name = ingredientDto.Name,
                    NormalizedName = ingredientDto.Name.ToNormal()
                },
                Amount = ingredientDto.Amount,
                UnitOfMeasure = uom
            };
            result.Add(recipeIngredient);
        }

        return result;
    }
}