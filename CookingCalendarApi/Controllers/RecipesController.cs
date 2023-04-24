using CookingCalendarApi.Enums;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using CookingCalendarApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("recipes")]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        //private Recipe ExampleRecipe = new Recipe(1, "Tacos", "Yummy Mexican Food", 12,
        //    new List<RecipeIngredient>()
        //    {
        //        new RecipeIngredient(1, 1, "Ground Beef", MeasurementType.Lb, 1, 1, 1, 2),
        //        new RecipeIngredient(2, 2, "Lettuce", MeasurementType.Cup, 1, 0, 1, 2),
        //        new RecipeIngredient(3, 3, "Cheese", MeasurementType.Cup, 1, 1, 0, 0),
        //        new RecipeIngredient(4, 4, "Taco Shells", MeasurementType.Amount, 1, 12, 0, 0),
        //    },
        //    new List<RecipeTag>()
        //    {
        //        new RecipeTag(1, 1, "Mexican",1),
        //        new RecipeTag(2, 2, "Easy", 2)
        //    },
        //    new List<RecipeStep>()
        //    {
        //        new RecipeStep(1, "Brown @1", 1,
        //        new List<RecipeStepIngredient>()
        //        {
        //            new RecipeStepIngredient(1, 1, null, 1, 1)
        //        }),
        //        new RecipeStep(2, "Get @1 and put @2 into each shell, top with @3 and @4 to taste", 1,
        //        new List<RecipeStepIngredient>()
        //        {
        //            new RecipeStepIngredient(2, 4, null, 1, 1),
        //            new RecipeStepIngredient(3, 1, null, 1, 12),
        //            new RecipeStepIngredient(4, 2, null, 1, 1),
        //            new RecipeStepIngredient(5, 3, null, 1, 1),

        //        }),
        //        new RecipeStep(3, "Enjoy", 1,new List<RecipeStepIngredient>()),
        //    }
        //    );

        public RecipesController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromBody] NewRecipe recipe)
        {
            try
            {
                if (recipe.Name == "")
                {
                    throw new InvalidDataException();
                }

                recipe.TrimAllStrings();

                return Created(nameof(GetRecipe), await _recipeRepository.AddRecipe(recipe, User.GetUserId()));
            }
            catch (InvalidDataException)
            {
                return BadRequest("Recipe is not in the correct format");
            }
        }

        [HttpGet("{recipeId}", Name = "GetRecipe")]
        public async Task<IActionResult> GetRecipe([FromRoute] int recipeId)
        {
            return Ok(await _recipeRepository.GetRecipe(recipeId));
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredRecipes([FromQuery] RecipeFilters filters)
        {
            return Ok(await _recipeRepository.GetRecipes(User.GetUserId()));
        }

        [HttpPut("{recipeId}")]
        public async Task<IActionResult> UpdateRecipe([FromRoute] int recipeId, [FromBody] Recipe recipe)
        {
            await _recipeRepository.UpdateRecipe(recipe);
            return Ok();
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            return Ok(await _recipeRepository.GetTags(User.GetUserId()));
        }

        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag([FromBody] NewTagDto tag)
        {
            return Ok(await _recipeRepository.CreateTag(tag, User.GetUserId()));
        }

        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> DeleteRecipe([FromRoute] int recipeId)
        {
            await _recipeRepository.DeleteRecipe(recipeId);
            return Ok();
        }
    }
}
