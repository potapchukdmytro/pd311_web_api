﻿using pd311_web_api.DAL.Entities;

namespace pd311_web_api.DAL.Repositories.JwtRepository
{
    public interface IJwtRepository
        : IGenericRepository<RefreshToken, string>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
