﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using pd311_web_api.BLL.DTOs.Role;
using pd311_web_api.BLL.Services.Role;

namespace pd311_web_api.Controllers
{
    [ApiController]
    [Route("api/role")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IValidator<RoleDto> _roleValidator;

        public RoleController(IRoleService roleService, IValidator<RoleDto> roleValidator)
        {
            _roleService = roleService;
            _roleValidator = roleValidator;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _roleService.GetAllAsync();
            return CreateActionResult(response); 
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(string? id)
        {
            if (!ValidateId(id, out string message))
                return BadRequest(message);

            var response = await _roleService.GetByIdAsync(id);
            return CreateActionResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RoleDto dto)
        {
            var validationResult = await _roleValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult);

            var response = await _roleService.CreateAsync(dto);
            return CreateActionResult(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] RoleDto dto)
        {
            var validationResult = await _roleValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(validationResult);

            if (!ValidateId(dto.Id, out string message))
                return BadRequest(message);

            var response = await _roleService.UpdateAsync(dto);
            return CreateActionResult(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string? id) 
        {
            if (!ValidateId(id, out string message))
                return BadRequest(message);

            var response = await _roleService.DeleteAsync(id);
            return CreateActionResult(response);
        }
    }
}
