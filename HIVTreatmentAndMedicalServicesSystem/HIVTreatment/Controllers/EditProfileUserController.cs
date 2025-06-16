using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires authentication
    public class EditProfileUserController : ControllerBase
    {
        private readonly IProfileService iProfileService;
        
        public EditProfileUserController(IProfileService profileService)
        {
            iProfileService = profileService;
        }
        
        [HttpPut("edit-profile")]
        public IActionResult EditProfile([FromBody] EditProfileUserDTO editProfileUserDTO)
        {
            if (editProfileUserDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            
            // Verify that the user is editing their own profile or is an admin
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            
            if (currentUserId != editProfileUserDTO.UserId && userRole != "R001")
            {
                return Forbid("You can only edit your own profile");
            }
            
            var result = iProfileService.UpdateProfile(editProfileUserDTO);
            if (result)
            {
                return Ok("Cập nhật hồ sơ thành công");
            }
            else
            {
                return NotFound("Không tìm thấy người dùng để cập nhật");
            }
        }
        
        // Example of a role-specific endpoint
        [HttpGet("admin-only")]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }
        
        // Another example for doctor role
        [HttpGet("doctor-only")]
        [Authorize(Policy = "RequireDoctorRole")]
        public IActionResult DoctorOnlyEndpoint()
        {
            return Ok("You are a doctor!");
        }
    }
}
