﻿using pd311_web_api.BLL.DTOs.Role;

namespace pd311_web_api.BLL.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public IEnumerable<RoleDto> Roles { get; set; } = [];
    }
}
