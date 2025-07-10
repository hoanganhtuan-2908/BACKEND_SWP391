using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HIVTreatment.Services;
using HIVTreatment.DTOs;
using Microsoft.AspNetCore.Http;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R004")]
    public class LabTestController : ControllerBase
    {
        private readonly ILabTestService _labTestService;

        public LabTestController(ILabTestService labTestService)
        {
            _labTestService = labTestService;
        }

        [HttpGet("AllLabTests")]
        public IActionResult GetAllLabTests()
        {
            var labTests = _labTestService.GetAllLabTests();
            return Ok(labTests);
        }
    }
}
