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

        // ===================== PATIENT =====================OOO====

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

            var isWorking = await _context.DoctorWorkSchedules.AnyAsync(w =>
                w.DoctorID == dto.DoctorID &&
                w.DateWork.Date == dto.BookDate.Date);

            if (!isWorking)
                return BadRequest("Bác sĩ không làm việc vào thời gian này.");

            var isConflict = await _context.BooksAppointments.AnyAsync(b =>
                b.DoctorID == dto.DoctorID &&
                b.BookDate.Date == dto.BookDate.Date &&
                b.Status == "Thành công");

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
            .ThenInclude(p => p.User)
            .Include(b => b.Doctor)
            .FirstOrDefaultAsync(b => b.BookID == appointment.BookID);

            return Ok(new
            {
                fullAppointment.BookID,
                fullAppointment.BookingType,
                fullAppointment.BookDate,
                fullAppointment.Status,
                fullAppointment.Note,
                PatientFullname = fullAppointment.Patient.User.Fullname,
                
            });
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

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (patient == null)
                return NotFound("Patient not found");

            var list = await _context.BooksAppointments
                .Where(a => a.PatientID == patient.PatientID)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate)
                .ToListAsync();

            var result = list.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Status,
                a.Note,
             
                a.Patient
              
            });

            return Ok(result);
        }


        // ===================== DOCTOR =========================
        // [PATIENT]  (Check-in)
        [HttpPut("PatientCheckin/{id}")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> PatientCheckIn(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null)
                return NotFound("Không tìm thấy bệnh nhân");

            var appointment = await _context.BooksAppointments
                .FirstOrDefaultAsync(a => a.BookID == id && a.PatientID == patient.PatientID);

            if (appointment == null)
                return NotFound("Không tìm thấy lịch hẹn");

            if (appointment.Status != "Thành công")
                return BadRequest("Chỉ có thể xác nhận với lịch hẹn ở trạng thái 'Thành công'.");

            appointment.Status = "Đã xác nhận";
            await _context.SaveChangesAsync();

            return Ok("Bệnh nhân đã xác nhận đến khám.");
        }


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

            if (appointment.Status != "Thành công")
                return BadRequest("Chỉ có thể huỷ lịch đã xác nhận.");

            appointment.Status = "Đã hủy";
            appointment.Note = reason;

            await _context.SaveChangesAsync();
            return Ok("Appointment cancelled.");
        }

        // [DOCTOR] CHECKOUT
        [HttpPut("DoctorCheckout/{id}")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> MarkAsCompleted(string id)
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null)
                return Unauthorized("Không tìm thấy bác sĩ");

            var appointment = await _context.BooksAppointments
                .FirstOrDefaultAsync(a => a.BookID == id && a.DoctorID == doctor.DoctorId);

            if (appointment == null)
                return NotFound("Không tìm thấy lịch hẹn");

            if (appointment.Status != "Đã xác nhận")
                return BadRequest("Bệnh nhân chưa check-in. Không thể tiến hành điều trị.");

            appointment.Status = "Đã khám";
            await _context.SaveChangesAsync();

            return Ok("Bác sĩ đã hoàn tất điều trị.");
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
                .Where(a => a.DoctorID == doctor.DoctorId && a.Status == "Thành công")
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate)
                .ToListAsync();

            var result = list.Select(a => new
            {   
                a.Doctor,
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Patient,
                a.Status
            });

            return Ok(result);
        }
        // [DOCTOR] Xem tất cả lịch hẹn của bệnh nhân do mình điều trị
        [HttpGet("mine-patients")]
        [Authorize(Roles = "R003")]
        public async Task<IActionResult> GetAppointmentsOfMyPatients()
        {
            var doctorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(doctorUserId))
                return Unauthorized("Doctor not logged in");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorUserId);
            if (doctor == null)
                return NotFound("Doctor not found");
            var appointments = await _context.BooksAppointments
                    .Where(a => a.DoctorID == doctor.DoctorId)
                    .Include(a => a.Patient)
                        .ThenInclude(p => p.User) // Lấy Fullname từ bảng Users
                    .OrderByDescending(a => a.BookDate)
                    .ToListAsync();

            var result = appointments.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Patient,
                a.Status,
                a.Note
            });

            return Ok(result);
        }


        // ===================== STAFF =========================

        // [STAFF] Xem tất cả lịch đã khám bao gồm cả đã khám và đã xác nhận
        [HttpGet("all")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> GetAllAppointmentsForStaff()
        {
            var list = await _context.BooksAppointments
                .Where(a => a.Status == "Thành công" || a.Status == "Đã hủy")
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .OrderByDescending(a => a.BookDate)
                .ToListAsync();

            var result = list.Select(a => new
            {
                a.BookID,
                PatientFullname = a.Patient.User.Fullname,
                a.BookingType,
                a.BookDate,
                a.Patient,
                a.Status
            });

            return Ok(result);
        }

    }
}
