using CookingCalendarApi.Enums;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("recipes")]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private Recipe ExampleRecipe = new Recipe(1, "Tacos", "Yummy Mexican Food", 12,
            new List<RecipeIngredient>()
            {
                new RecipeIngredient(1, 1, "Ground Beef", MeasurementType.Lb, 1, 1, 1, 2),
                new RecipeIngredient(2, 2, "Lettuce", MeasurementType.Cup, 1, 0, 1, 2),
                new RecipeIngredient(3, 3, "Cheese", MeasurementType.Cup, 1, 1, 0, 0),
                new RecipeIngredient(4, 4, "Taco Shells", MeasurementType.Amount, 1, 12, 0, 0),
            },
            new List<RecipeTag>()
            {
                new RecipeTag(1, 1, "Mexican"),
                new RecipeTag(2, 2, "Easy")
            },
            new List<RecipeStep>()
            {
                new RecipeStep(1, "Brown @1", 1,
                new List<RecipeStepIngredient>()
                {
                    new RecipeStepIngredient(1, 1, null, 1, 1)
                }),
                new RecipeStep(2, "Get @1 and put @2 into each shell, top with @3 and @4 to taste", 1,
                new List<RecipeStepIngredient>()
                {
                    new RecipeStepIngredient(2, 4, null, 1, 1),
                    new RecipeStepIngredient(3, 1, null, 1, 12),
                    new RecipeStepIngredient(4, 2, null, 1, 1),
                    new RecipeStepIngredient(5, 3, null, 1, 1),

                }),
                new RecipeStep(3, "Enjoy", 1,new List<RecipeStepIngredient>()),
            }
            );
        
        [HttpPost]
        public IActionResult CreateRecipe([FromBody] Recipe recipe)
        {
            try
            {
                if (recipe.Name == "")
                {
                    throw new InvalidDataException();
                }

                return Created(nameof(GetRecipe), ExampleRecipe);
                //return Ok(1);
            }
            catch (InvalidDataException)
            {
                return BadRequest("Recipe is not in the correct format");
            }
        }

        [HttpGet("{recipeId}", Name = "GetRecipe")]
        public IActionResult GetRecipe([FromRoute] int recipeId)
        {
            try
            {
                if (recipeId != 1)
                {
                    throw new InvalidDataException();
                }

                return Ok(ExampleRecipe);
            }
            catch (InvalidDataException)
            {
                return BadRequest("Recipe with given Id could not be found");
            }
        }

        [HttpGet]
        public IActionResult GetFilteredRecipes([FromQuery] RecipeFilters filters)
        {
            if(filters.Limit == 0)
            {
                return BadRequest("Limit is required");
            }

            //return NoContent();
            return Ok(new RecipeReferenceInfo(1, "Tacos", new List<string>() { "Mexican" }, new List<string>()
            {
                "Ground Beef", "Lettuce", "Cheese", "Taco Shells",
            }));
        }


        [HttpPut("{recipeId}")]
        public IActionResult UpdateRecipe([FromRoute] int recipeId, [FromBody] Recipe recipe)
        {
            try
            {
                if (recipeId != 1)
                {
                    throw new InvalidDataException();
                }

                //                recipe = new Recipe(null, "Tacos", "Yummy Mexican Food", 12,
                //new List<RecipeIngredient>()
                //{
                //                    new RecipeIngredient(null, 1, "Ground Beef", MeasurementType.Lb, 1, 1, 1, 2),
                //                    new RecipeIngredient(null, 2, "Lettuce", MeasurementType.Cup, 1, 0, 1, 2),
                //                    new RecipeIngredient(null, 3, "Cheese", MeasurementType.Cup, 1, 1, 0, 0),
                //                    new RecipeIngredient(null, 4, "Taco Shells", MeasurementType.Amount, 1, 12, 0, 0),
                //},
                //new List<RecipeTag>()
                //{
                //                    new RecipeTag(null, 1, "Mexican"),
                //                    new RecipeTag(null, 2, "Easy")
                //},
                //new List<RecipeStep>()
                //{
                //                    new RecipeStep(null, "Brown @1", 1,
                //                    new List<RecipeStepIngredient>()
                //                    {
                //                        new RecipeStepIngredient(null, 1, null, 1, 1)
                //                    }),
                //                    new RecipeStep(null, "Get @1 and put @2 into each shell, top with @3 and @4 to taste", 1,
                //                    new List<RecipeStepIngredient>()
                //                    {
                //                        new RecipeStepIngredient(null, null, 4, 1, 1),
                //                        new RecipeStepIngredient(null, 1, null, 1, 12),
                //                        new RecipeStepIngredient(null, 2, null, 1, 1),
                //                        new RecipeStepIngredient(null, 3, null, 1, 1),

                //                    }),
                //                    new RecipeStep(null, "Enjoy", 1, new List<RecipeStepIngredient>()),
                //}
                //);

                return Ok(recipe);
            }
            catch (InvalidDataException)
            {
                return BadRequest("Recipe is not in the correct format");
            }
        }

        [HttpDelete("{recipeId}")]
        public IActionResult DeleteRecipe([FromRoute] int recipeId)
        {
            try
            {
                if (recipeId != 1)
                {
                    throw new InvalidDataException();
                }

                return Ok(1);
            }
            catch (InvalidDataException)
            {
                return BadRequest("Recipe can not be deleted");
            }
        }
    }
}
