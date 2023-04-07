using CookingCalendarApi.Enums;
using CookingCalendarApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CookingCalendarApi.Controllers
{
    [ApiController]
    [Route("shopping")]
    public class ShoppingController : ControllerBase
    {
        private ShoppingList _shoppingList = new ShoppingList(1, new DateOnly(2023, 3, 23), new DateOnly(2023, 3, 31), DateTime.Now,
            new List<GeneratedItem>()
            {
                new GeneratedItem(1, false, 1, "Ground Beef", 3, MeasurementType.Lb),
                new GeneratedItem(2, true, 1, "Cheddar Cheese", 12, MeasurementType.Oz)
            },
            new List<EnteredItem>()
            {
                new EnteredItem(1, false, "Mountain Dew", ShoppingCategory.Drink),
                new EnteredItem(2, true, "Tortilla Chips", ShoppingCategory.Snack),
            }
        );

        [HttpGet(Name = "ShoppingList")]
        public IActionResult GetShoppingList()
        {
            try
            {
                if (false)
                {
                    throw new InvalidDataException();
                }

                return Ok(_shoppingList);
            }
            catch (InvalidDataException)
            {
                return BadRequest("Recipe with given Id could not be found");
            }
        }

        [HttpPost]
        public IActionResult GenerateShoppingList([FromBody] DateFilter filters)
        {
            if (filters.StartDate.CompareTo(filters.EndDate) > 0)
            {
                return BadRequest("Start date must come before end date");
            }
            return Created(nameof(GetShoppingList), _shoppingList);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateShoppingList([FromRoute] int id, [FromBody] object updates)
        {
            //if (updates.EnteredItemsToUpdate != null && updates.EnteredItemsToUpdate.Any(x => x.Id == -1))
            //{
            //    return BadRequest("Entered Item Id did not exist");
            //}
            var test = JsonSerializer.Deserialize<ShoppingList>(updates.ToString());
            return Ok(new ShoppingListUpdate(1, new List<int>() { 1, 2 }, new List<int>() { 3}, new List<int>() { 4 }, new List<EnteredItem>() { new EnteredItem(1, true, "Mountain Dew", ShoppingCategory.Drink)}, new List<int>() { 1 }));
            //return NoContent();
        }
    }
}
