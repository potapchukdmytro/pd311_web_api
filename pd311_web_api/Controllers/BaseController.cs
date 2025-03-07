﻿using Microsoft.AspNetCore.Mvc;

namespace pd311_web_api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected bool ValidateId(string? id, out string message)
        {
            if (string.IsNullOrEmpty(id))
            {
                message = "Id is required";
                return false;
            }

            bool parseResult = Guid.TryParse(id, out Guid res);
            if (!parseResult)
            {
                message = "Id incorrect format";
                return false;
            }

            message = "Id is valid";
            return true;
        }
    }
}
