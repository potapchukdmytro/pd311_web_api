﻿using pd311_web_api.BLL.DTOs.Account;
using static pd311_web_api.DAL.Entities.IdentityEntities;

namespace pd311_web_api.BLL.Services.Account
{
    public interface IAccountService
    {
        public Task<AppUser?> LoginAsync(LoginDto dto);
        public Task<AppUser?> RegisterAsync(RegisterDto dto);
        public Task<bool> ConfirmEmailAsync(string id, string token);
    }
}
