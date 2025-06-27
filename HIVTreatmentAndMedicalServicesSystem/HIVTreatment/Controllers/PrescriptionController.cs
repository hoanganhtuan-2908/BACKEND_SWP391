using HIVTreatment.DTOs;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService prescriptionService;
        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            this.prescriptionService = prescriptionService;
        }
        [HttpPost("add-prescription")]
        public IActionResult AddPrescription([FromBody] PrescriptionDTO prescriptionDto)
        {

            if (prescriptionDto == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền kê đơn thuốc!");
            }
            var result = prescriptionService.AddPrescription(prescriptionDto);
            if (result)
            {
                return Ok("Kê đơn thuốc thành công");
            }
            else
            {
                return BadRequest("Kê đơn thuốc không thành công");
            }
        }
    }
}


