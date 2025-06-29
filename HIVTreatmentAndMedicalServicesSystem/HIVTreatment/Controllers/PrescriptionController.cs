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
        [HttpPut("update-prescription")]
        public IActionResult UpdatePrescription([FromBody] UpdatePrescriptionDTO prescriptionDto)
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
                return Forbid("Bạn không có quyền cập nhật đơn thuốc!");
            }
            var result = prescriptionService.UpdatePrescription(prescriptionDto);
            if (result)
            {
                return Ok("Cập nhật đơn thuốc thành công");
            }
            else
            {
                return BadRequest("Cập nhật đơn thuốc không thành công");
            }
        }

        [HttpGet("get-all-prescriptions")]
        public IActionResult GetAllPrescriptions()
        {
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return BadRequest("Bạn không có quyền xem đơn thuốc!");
            }
            var prescriptions = prescriptionService.GetAllPrescription();
            return Ok(prescriptions);

        }

        [HttpGet("get-prescription-by-id/{prescriptionId}")]
        public IActionResult GetPrescriptionById(string prescriptionId)
        {
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return BadRequest("Bạn không có quyền xem đơn thuốc!");
            }
            var prescription = prescriptionService.GetPrescriptionById(prescriptionId);
            if (prescription == null)
            {
                return NotFound("Không tìm thấy đơn thuốc với ID đã cho.");
            }
            return Ok(prescription);
        }
        [HttpGet("get-prescription-by-patient-and-doctor/{medicalRecordId}")]
        public IActionResult GetPrescriptionByPatientAndDoctor(string medicalRecordId, string doctorId)
        {
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return BadRequest("Bạn không có quyền xem đơn thuốc!");
            }
            var prescriptions = prescriptionService.GetPrescriptionByPatientAndDoctor(medicalRecordId, doctorId);
            return Ok(prescriptions);
        }
    }
}


