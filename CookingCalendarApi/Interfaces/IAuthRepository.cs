﻿using CookingCalendarApi.Models;

namespace CookingCalendarApi.Interfaces
{
    public interface IAuthRepository
    {
        Task<int> Login(User user);
        Task<int?> CreateUser(User user);
        Task<int> ChangePassword(ChangePasswordDto user);
        Task<IEnumerable<string>> GetUsernamesInUse();
    }
}
