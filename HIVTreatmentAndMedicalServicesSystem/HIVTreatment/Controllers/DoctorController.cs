using HIVTreatment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        [HttpGet("AllARVProtocol")]
        public IActionResult GetAllARVRegiemns()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return
                Forbid("Bạn không có quyền xem ARV Protocol!");
            }
            var arvProtocols = _doctorService.GetAllARVRegimens();
            if (arvProtocols == null || !arvProtocols.Any())
            {
                return NotFound("Không có phác đồ ARV nào.");
            }
            return Ok(arvProtocols);
        }

        [HttpGet("AllDoctors")]
        public IActionResult GetAllDoctors()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem danh sách bác sĩ!");
            }
            var doctors = _doctorService.GetAllDoctors();
            if (doctors == null || !doctors.Any())
            {
                return NotFound("Không có bác sĩ nào.");
            }
            return Ok(doctors);
        }

        [HttpGet("InfoDoctor/{doctorId}")]
        public IActionResult GetInfoDoctorById(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin bác sĩ!");
            }
            var doctorInfo = _doctorService.GetInfoDoctorById(doctorId);
            if (doctorInfo == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");
            }
            return Ok(doctorInfo);
        }
    }
}
