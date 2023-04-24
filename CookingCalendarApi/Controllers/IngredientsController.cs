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
    [Authorize]
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
            return Ok(await _ingredientRepository.GetIngredients(User.GetUserId()));
        }


        [HttpPost]
        public async Task<IActionResult> CreateIngredient([FromBody] Ingredient ing)
        {
            ing.TrimAllStrings();
            var id = await _ingredientRepository.AddIngredient(ing);
            await _ingredientRepository.AddUserIngredient(User.GetUserId(), id);
            return Ok(id);
        }

        [HttpPost("user")]
        public async Task<IActionResult> CreateUserIngredient([FromBody] IdDto dto)
        {
            return Ok(await _ingredientRepository.AddUserIngredient(User.GetUserId(), dto.Id));
        }
    }
}
