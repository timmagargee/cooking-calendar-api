using CookingCalendarApi.Enums;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using CookingCalendarApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientsController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredients()
        {
            return Ok(await _ingredientRepository.GetIngredients());
        }


        [HttpPost]
        public async Task<IActionResult> CreateIngredient([FromBody] Ingredient ing)
        {
            ing.TrimAllStrings();
            var id = await _ingredientRepository.AddIngredient(ing);
            return Ok(id);
        }
    }
}
