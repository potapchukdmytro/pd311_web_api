﻿using Microsoft.AspNetCore.Mvc;
using pd311_web_api.BLL.DTOs.Car;
using pd311_web_api.BLL.Services.Cars;

namespace pd311_web_api.Controllers
{
    [ApiController]
    [Route("api/car")]
    public class CarController : BaseController
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCarDto dto)
        {
            var response = await _carService.CreateAsync(dto);
            return CreateActionResult(response);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _carService.GetAllAsync();
            return CreateActionResult(response);
        }
    }
}
