using HIVTreatment.Repositories;
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

        public TreatmentPlanController(ITreatmentPlanRepository repository)
        {
            _repository = repository;
        }

        // Admin xem toàn bộ
        [Authorize(Roles = "R001")]
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

    }
}
