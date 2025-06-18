using HIVTreatment.Data;
using HIVTreatment.Models;
using HIVTreatment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _repository;
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context, IAppointmentRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // STAFF: Xem tất cả lịch hẹn
        [HttpGet]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repository.GetAllAsync();
            return Ok(list);
        }

        // STAFF: Xem chi tiết lịch hẹn
        [HttpGet("{id}")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // PATIENT: Tạo lịch hẹn mới
        [HttpPost]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> Create([FromBody] BookAppointmentDTO dto)
        {
            var userId = User.FindFirst("PatientID")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Patient not logged in");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null)
                return NotFound("Patient not found");

            var book = new BooksAppointment
            {
                BookID = "BK" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(),
                PatientID = patient.PatientID,
                DoctorID = dto.DoctorID,
                ServiceID = dto.ServiceID,
                BookDate = dto.BookDate,
                Status = "Đang chờ",
                Note = dto.Note
            };

            _context.BooksAppointments.Add(book);
            await _context.SaveChangesAsync();

            return Ok(book);
        }

        // PATIENT: Xem lịch hẹn của chính mình
        [HttpGet("mine")]
        [Authorize(Roles = "R005")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = User.FindFirst("PatientID")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Patient not logged in");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
            if (patient == null)
                return NotFound("Patient not found");

            var appointments = await _context.BooksAppointments
                .Where(a => a.PatientID == patient.PatientID)
                .ToListAsync();

            return Ok(appointments);
        }

        // STAFF: Cập nhật trạng thái lịch hẹn
        [HttpPut("{id}/status")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] string newStatus)
        {
            var appointment = await _context.BooksAppointments.FindAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found");

            appointment.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        // STAFF: Xoá lịch hẹn nếu đang chờ hoặc từ chối
        [HttpDelete("{id}")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> Delete(string id)
        {
            var appointment = await _context.BooksAppointments.FindAsync(id);
            if (appointment == null)
                return NotFound("Appointment not found");

            if (appointment.Status != "Đang chờ" && appointment.Status != "Từ chối")
                return BadRequest("Chỉ được xoá lịch hẹn chưa duyệt hoặc đã bị từ chối.");

            _context.BooksAppointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok($"Đã xoá lịch hẹn {id}");
        }
        [HttpPut("{id}/approval")]
        [Authorize(Roles = "R004")]
        public async Task<IActionResult> ApproveAppointment(string id, [FromBody] AppointmentApprovalDTO dto)
        {
            var appointment = await _context.BooksAppointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.BookID == id);

            if (appointment == null)
                return NotFound("Không tìm thấy lịch hẹn.");

            if (appointment.Status != "Đang chờ")
                return BadRequest("Chỉ có thể duyệt lịch hẹn đang chờ.");

            if (dto.Status == "Đã xác nhận")
            {
                appointment.Status = "Đã xác nhận";
                

                // Gửi thông báo xác nhận
                Console.WriteLine($"✅ Lịch hẹn {id} đã xác nhận cho bệnh nhân {appointment.PatientID} và bác sĩ {appointment.DoctorID} vào ngày {appointment.BookDate}");
            }
            else if (dto.Status == "Từ chối")
            {
                
            }
            else
            {
                return BadRequest("Trạng thái không hợp lệ. Chỉ chấp nhận 'Đã xác nhận' hoặc 'Từ chối'.");
            }

            await _context.SaveChangesAsync();
            return Ok(appointment);
        }
    }
}
