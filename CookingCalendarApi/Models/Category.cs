﻿using CookingCalendarApi.Enums;

namespace CookingCalendarApi.Models
{
    public class Category
    {
        public Category(int? id, string? name, DayOfWeek dayOfWeek, CategoryType categoryType, int? tagId, int? ingredientId)
        {
            Id = id;
            Name = name;
            DayOfWeek = dayOfWeek;
            CategoryType = categoryType;
            TagId = tagId;
            IngredientId = ingredientId;
        }

        public int? Id { get; set; }
        public string? Name { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public CategoryType CategoryType { get; set; }
        public int? TagId { get; set; }
        public int? IngredientId { get; set; }
    }
}
