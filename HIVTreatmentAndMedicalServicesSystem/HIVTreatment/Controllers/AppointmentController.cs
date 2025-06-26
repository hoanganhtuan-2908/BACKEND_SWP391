using HIVTreatment.Data;
using HIVTreatment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== PATIENT =========================

        // [PATIENT] Đặt lịch hẹn
        [HttpPost("booking")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
        {
            var userId = User.FindFirst("PatientID")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Patient not logged in");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null)
                return NotFound("Patient not found");

            // Bỏ kiểm tra slot vì database không có SlotID
            var isWorking = await _context.DoctorWorkSchedules.AnyAsync(w =>
                w.DoctorID == dto.DoctorID &&
                w.DateWork.Date == dto.BookDate.Date);

            if (!isWorking)
                return BadRequest("Bác sĩ không làm việc vào thời gian này.");

            var isConflict = await _context.BooksAppointments.AnyAsync(b =>
                b.DoctorID == dto.DoctorID &&
                b.BookDate.Date == dto.BookDate.Date &&
                b.Status == "Đã xác nhận");

            if (isConflict)
                return Conflict("Bác sĩ đã có lịch hẹn trong ngày này.");

            var appointment = new BooksAppointment
            {
                BookID = "BK" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                PatientID = patient.PatientID,
                DoctorID = dto.DoctorID,
                BookingType = dto.BookingType,
                BookDate = dto.BookDate,
                Status = "Thành công",
                Note = dto.Note
            };

            _context.BooksAppointments.Add(appointment);
            await _context.SaveChangesAsync();
            var fullAppointment = await _context.BooksAppointments
            .Include(b => b.Patient)
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.BookID == appointment.BookID);

            return Ok(appointment);
        }
        // [PATIENT] Hủy lịch hẹn của chính mình
        [HttpPut("rejected/{id}")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> CancelAppointmentByPatient(string id, [FromBody] string reason)
        {
            var userId = User.FindFirst("PatientID")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Patient not logged in");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null)
                return NotFound("Patient not found");

            var appointment = await _context.BooksAppointments
                .FirstOrDefaultAsync(a => a.BookID == id && a.PatientID == patient.PatientID);

            if (appointment == null)
                return NotFound("Appointment not found");

            if (appointment.Status != "Đã xác nhận" && appointment.Status != "Thành công")
                return BadRequest("Chỉ có thể huỷ lịch đã xác nhận hoặc thành công.");

            appointment.Status = "Đã hủy";
            appointment.Note = reason;

            await _context.SaveChangesAsync();
            return Ok("Appointment cancelled by patient.");
        }

        // [PATIENT] Xem lịch hẹn của mình
        [HttpGet("mine")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = User.FindFirst("PatientID")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null)
                return NotFound("Patient not found");

            var list = await _context.BooksAppointments
                .Where(a => a.PatientID == patient.PatientID)
                .ToListAsync();

            return Ok(list);
        }

        // ===================== DOCTOR =========================

        // [DOCTOR] Hủy lịch hẹn
        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> CancelAppointment(string id, [FromBody] string reason)
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null)
                return Unauthorized("Doctor not found");

            var appointment = await _context.BooksAppointments
                .FirstOrDefaultAsync(a => a.BookID == id && a.DoctorID == doctor.DoctorId);

            if (appointment == null)
                return NotFound("Appointment not found");

            if (appointment.Status != "Đã xác nhận")
                return BadRequest("Chỉ có thể huỷ lịch đã xác nhận.");

            appointment.Status = "Bị từ chối";
            appointment.Note = reason;

            await _context.SaveChangesAsync();
            return Ok("Appointment cancelled.");
        }

        // [DOCTOR] Đánh dấu đã khám
        [HttpPut("complete/{id}")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> MarkAsCompleted(string id)
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null)
                return Unauthorized("Doctor not found");

            var appointment = await _context.BooksAppointments
                .FirstOrDefaultAsync(a => a.BookID == id && a.DoctorID == doctor.DoctorId);

            if (appointment == null)
                return NotFound("Appointment not found");

            if (appointment.Status != "Đã xác nhận")
                return BadRequest("Chỉ có thể đánh dấu đã khám với lịch đã xác nhận.");

            appointment.Status = "Đã khám";
            await _context.SaveChangesAsync();
            return Ok("Appointment marked as completed.");
        }

        // [DOCTOR] Xem danh sách lịch đã xác nhận
        [HttpGet("approved")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null)
                return Unauthorized("Doctor not found");

            var list = await _context.BooksAppointments
                .Where(a => a.DoctorID == doctor.DoctorId && a.Status == "Đã xác nhận")
                .ToListAsync();

            return Ok(list);
        }

        // ===================== STAFF =========================

        // [STAFF] Xem tất cả lịch đã khám
        [HttpGet("all")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> GetAllAppointmentsForStaff()
        {
            var list = await _context.BooksAppointments
                .Where(a => a.Status == "Thành công" || a.Status == "Đã hủy")
                .ToListAsync();

            return Ok(list);
        }
    }
}
