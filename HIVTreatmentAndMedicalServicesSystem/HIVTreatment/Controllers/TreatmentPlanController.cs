using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentPlanController : ControllerBase
    {
        private readonly ITreatmentPlanRepository _repository;
        private readonly ITreatmentPlan treatmentPlan;

        public TreatmentPlanController(ITreatmentPlanRepository repository, ITreatmentPlanRepository TreatmentRepo)
        {
            _repository = repository;
            treatmentPlan = new TreatmentPlan(TreatmentRepo);
        }

        // Admin xem toàn bộ
        [Authorize(Roles = "R001, R004")]
        [HttpGet("admin")]
        public IActionResult GetAll()
        {
            var plans = _repository.GetAll();
            return Ok(plans);
        }

        // Doctor chỉ xem treatment plan của họ
        [Authorize(Roles = "R003")]
        [HttpGet("doctor")]
        public IActionResult GetByDoctor()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Doctor not logged in");

            var plans = _repository.GetByDoctorUserId(userId);
            return Ok(plans);
        }
        //admin xem được treatment plan theo patientId,doctor xem đc của chính bệnh nhân họ điều trị
        [HttpGet("by-patient/{patientId}")]
        [Authorize(Roles = "R001,R003")]
        public IActionResult GetByPatient(string patientId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "R001")  // Admin
            {
                var plans = _repository.GetByPatient(patientId);
                return Ok(plans);
            }
            else if (role == "R003")  // Doctor
            {
                var doctorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var plans = _repository.GetByPatientAndDoctor(patientId, doctorUserId);
                return Ok(plans);
            }

            return Forbid();
        }

        [HttpPost("AddTreatmentPlan")]
        public IActionResult AddTreatmentPlan([FromBody] TreatmentPlanDTO treatmentPlanDTO)
        {
            if (treatmentPlanDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (currentUserId != treatmentPlanDTO.DoctorID && !allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền thêm kế hoạch điều trị cho người khác");
            }
            var result = treatmentPlan.AddTreatmentPlan(treatmentPlanDTO);
            if (result)
            {
                return Ok("Thêm kế hoạch điều trị thành công");
            }
            else
            {
                return BadRequest("Thêm kế hoạch điều trị thất bại");
            }

        }
        [HttpPut("UpdateTreatmentPlan")]
        public IActionResult UpdateTreatmentPlan([FromBody] UpdateTreatmentPlanDTO updateTreatmentPlanDTO)
        {
            if (updateTreatmentPlanDTO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            // Lấy thông tin người dùng từ JWT
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            // Kiểm tra quyền
            var allowedRoles = new[] { "R001", "R003" };
            if (currentUserId != updateTreatmentPlanDTO.DoctorID && !allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền cập nhật kế hoạch điều trị cho người khác");
            }
            var result = treatmentPlan.UpdateTreatmentPlan(updateTreatmentPlanDTO);
            if (result)
            {
                return Ok("Cập nhật kế hoạch điều trị thành công");
            }
            else
            {
                return BadRequest("Cập nhật kế hoạch điều trị thất bại");
            }
        }

    }
}
