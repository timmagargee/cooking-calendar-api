using CookingCalendarApi.DomainModels;
using CookingCalendarApi.Enums;
using CookingCalendarApi.Extensions;
using CookingCalendarApi.Models;
using CookingCalendarApi.Repositories;
using CookingCalendarApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("shopping")]
    public class ShoppingController : ControllerBase
    {
        private readonly IShoppingRepository _shoppingRepository;
        private readonly ISettingsRepository _settingsRepository;

        public ShoppingController(IShoppingRepository shoppingRepository, ISettingsRepository settingsRepository)
        {
            _shoppingRepository = shoppingRepository;
            _settingsRepository = settingsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetShoppingList()
        {
            return Ok(await _shoppingRepository.GetUserShoppingList(User.GetUserId()));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateShoppingList([FromBody] DateFilter filters)
        {
            if (filters.StartDate.CompareTo(filters.EndDate) > 0)
            {
                return BadRequest("Start date must come before end date");
            }

            //var generator = new ShoppingListGenerator(new List<ShoppingIngredient>() { new ShoppingIngredient() {
            //    IngredientId = 1,
            //    Amount = new RecipeAmount(){ Amount = 1, Measurement = MeasurementType.Tsp}
            //} });
            var userId = User.GetUserId();
            var settings = await _settingsRepository.GetUserSettings(userId);
            var ingredients = await _shoppingRepository.GetShoppingIngredients(userId, filters);
            var generator = new ShoppingListGenerator(ingredients);
            var items = generator.CreateShoppingList(settings.DefaultServings).OrderBy(x => x.IngredientId);

            var shoppingListId = await _shoppingRepository.AddOrUpdateShoppingList(userId, filters);
            await _shoppingRepository.DeleteGeneratedItems(shoppingListId);
            await _shoppingRepository.AddGeneratedItems(shoppingListId, items);
            return Ok();
            //return Created(nameof(GetShoppingList), _shoppingList);
        }


        [HttpPut("")]
        public async Task<IActionResult> UpdateShoppingList([FromBody] ShoppingListDto list)
        {
            await _shoppingRepository.UpdateShoppingList(list);
            return Ok();
        }

        [HttpDelete("{id}/checked")]
        public async Task<IActionResult> ClearCheckedItems([FromRoute] int id)
        {
            await _shoppingRepository.ClearChecked(id);
            return Ok();
        }
    }
}
