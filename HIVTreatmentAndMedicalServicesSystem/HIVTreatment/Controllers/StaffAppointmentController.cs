using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/staff/appointments")]
    public class StaffAppointmentController : ControllerBase
    {
        private readonly StaffAppointmentService _appointmentService;

        public StaffAppointmentController(StaffAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        private User GetCurrentUser()
        {
            return _appointmentService.GetCurrentUser(User);
        }

        /// <summary>
        /// Danh sách lịch đã khám
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("completed")]
        public IActionResult GetCompletedAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !_appointmentService.IsStaff(user))
                return Forbid();

            var appointments = _appointmentService.GetCompletedAppointments();
            return Ok(appointments);
        }

        /// <summary>
        /// Danh sách lịch đã đặt (Thành công)
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("successful")]
        public IActionResult GetSuccessfulAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !_appointmentService.IsStaff(user))
                return Forbid();

            var appointments = _appointmentService.GetSuccessfulAppointments();
            return Ok(appointments);
        }

        /// <summary>
        /// Danh sách lịch đã hủy
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("cancelled")]
        public IActionResult GetCancelledAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !_appointmentService.IsStaff(user))
                return Forbid();

            var appointments = _appointmentService.GetCancelledAppointments();
            return Ok(appointments);
        }

        /// <summary>
        /// Tất cả lịch hẹn: Thành công + Đã hủy + Đã khám
        /// </summary>
        [Authorize(Roles = "R004")]
        [HttpGet("all")]
        public IActionResult GetAllAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !_appointmentService.IsStaff(user))
                return Forbid();

            var appointments = _appointmentService.GetAllRelevantAppointments();
            return Ok(appointments);
        }
    }
}
