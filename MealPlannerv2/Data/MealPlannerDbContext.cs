using MealPlannerv2.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MealPlannerv2.Data;

public class MealPlannerDbContext: DbContext
{
    public MealPlannerDbContext(DbContextOptions<MealPlannerDbContext> opts): base(opts)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecipeIngredient>().HasKey(x => new { x.IngredientId, x.RecipeId });
    }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<UnitOfMeasure> UnitOfMeasure { get; set; }
}