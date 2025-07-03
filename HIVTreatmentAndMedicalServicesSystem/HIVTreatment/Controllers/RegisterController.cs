using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public RegisterController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return BadRequest("Invalid registration data");
            }

            // Validate required fields
            if (string.IsNullOrEmpty(registerDTO.Fullname) ||
                string.IsNullOrEmpty(registerDTO.Password) ||
                string.IsNullOrEmpty(registerDTO.Email))
            {
                return BadRequest("All fields are required");
            }

            // Get default role from configuration or use fallback
            string defaultRole = _configuration["DefaultUserRole"] ?? "R005";

            try
            {
                // Convert DTO to User model
                var user = new User
                {
                    RoleId = defaultRole,
                    Fullname = registerDTO.Fullname,
                    Password = registerDTO.Password,
                    Email = registerDTO.Email,
                    Address = registerDTO.Address,
                    Image = "patient.png"

                    // UserId will be assigned in the service
                };

                var result = _userService.Register(user);
                if (result == null)
                {
                    return BadRequest("Email đã tồn tại hoặc thông tin không hợp lệ");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred during registration");
            }
        }
    }
}
