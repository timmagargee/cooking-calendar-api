﻿namespace CookingCalendarApi.Models
{
    public class ChangePasswordDto
    {
        public int? UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
