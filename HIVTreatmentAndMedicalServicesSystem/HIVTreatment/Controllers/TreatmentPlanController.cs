using HIVTreatment.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = "R001,R003")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var plans = _repository.GetAll();
            return Ok(plans);
        }

        [HttpGet("{patientId}")]
        public IActionResult GetByPatient(string patientId)
        {
            var plans = _repository.GetByPatient(patientId);
            return Ok(plans);
        }
    }
}
