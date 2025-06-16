using HIVTreatment.Data;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/staff/appointments")]
    public class StaffAppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notifier;

        public StaffAppointmentController(ApplicationDbContext context, INotificationService notifier)
        {
            _context = context;
            _notifier = notifier;
        }

        private User GetCurrentUser()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        private bool IsStaff(User user) => user.RoleId == "R004";
        [Authorize(Roles = "R004")]

        [HttpGet("pending")]
        public IActionResult GetPendingAppointments()
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();

            var pendingAppointments = _context.BooksAppointments
                .Where(a => a.Status == "Đang chờ")
                .ToList();

            return Ok(pendingAppointments);
        }

        [HttpPut("approve/{id}")]
        public IActionResult ApproveAppointment(string id)
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();
            //var User = new User { UserId = "TEST001", RoleId = "R004" };

            var appointment = _context.BooksAppointments.FirstOrDefault(a => a.BookID == id);
            if (appointment == null)
                return NotFound();

            appointment.Status = "Đã xác nhận";
            _context.SaveChanges();

            // Gửi thông báo
            _notifier.Send(appointment.PatientID, "Lịch hẹn của bạn đã được duyệt.");
            _notifier.Send(appointment.DoctorID, $"Bạn có lịch hẹn mới vào {appointment.BookDate:dd/MM/yyyy HH:mm}");

            return Ok("Appointment approved");
        }

        [HttpPut("reject/{id}")]
        public IActionResult RejectAppointment(string id, [FromBody] RejectDto dto)
        {
            var user = GetCurrentUser();
            if (user == null || !IsStaff(user))
                return Forbid();

            var appointment = _context.BooksAppointments.FirstOrDefault(a => a.BookID == id);
            if (appointment == null)
                return NotFound();

            appointment.Status = "Rejected";
            appointment.Note = dto.Reason;
            _context.SaveChanges();

            _notifier.Send(appointment.PatientID, $"Lịch hẹn của bạn đã bị từ chối. Lý do: {dto.Reason}");

            return Ok("Appointment rejected");
        }
    }

    public class RejectDto
    {
        public string Reason { get; set; }
    }
}
