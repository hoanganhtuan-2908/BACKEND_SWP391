using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HIVTreatment.Models;
using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HIVTreatment.Services;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001,R002")]
 

    public class ManagerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorService _doctorService;

        public ManagerController(ApplicationDbContext context, IUserRepository userRepository, IDoctorService doctorService)
        {
            _context = context;
            _userRepository = userRepository;
            _doctorService = doctorService;
        }


        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] CreateDoctorDTO dto)
        {
            // kiểm tra email đã tồn tại chưa
            if (_userRepository.EmailExists(dto.Email))
            {
                return BadRequest("Email đã tồn tại.");
            }

            // Tạo UserId
            var lastUser = _userRepository.GetLastUser();
            int nextUserId = 1;
            if (lastUser != null && int.TryParse(lastUser.UserId.Substring(3), out int lastId))
            {
                nextUserId = lastId + 1;
            }
            string newUserId = $"UI{nextUserId:D6}";


            //Tạo User mới
            var user = new User
            {
                UserId = newUserId,
                RoleId = "R003", //Doctor
                Fullname = dto.FullName,
                Password = dto.Password,
                Email = dto.Email
            };

            try
            {
                _userRepository.Add(user); // Lưu User vào database
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Tạo user thất bại: {ex.InnerException?.Message ?? ex.Message}");
            }

            // Tạo DoctorId
            // DT + 6 số
            var lastDoctor = _context.Doctors.OrderByDescending(d => d.DoctorId).FirstOrDefault();
            int nextDoctorId = 1;
            if (lastDoctor != null && int.TryParse(lastDoctor.DoctorId.Substring(2), out int lastDId))
            {
                nextDoctorId = lastDId + 1;
            }
            string newDoctorId = $"DT{nextDoctorId:D6}";

            // Tạo Doctor mới và gán UserId vừa tạo
            var doctor = new Doctor
            {
                DoctorId = newDoctorId,
                UserId = newUserId, // Gán UserId từ User vừa tạo
                Specialization = dto.Specialization,
                LicenseNumber = dto.LicenseNumber,
                ExperienceYears = dto.ExperienceYears
            };

            try
            {
                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Thêm bác sĩ mới thất bại: {ex.Message}");
            }

            return Ok(new { message = "Thêm bác sĩ thành công.", doctorId = newDoctorId, userId = newUserId });
        }

        [HttpGet("AllDoctors")]
        public IActionResult GetAllDoctors()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem danh sách bác sĩ!");
            }
            var doctors = _doctorService.GetAllDoctors();
            if (doctors == null || !doctors.Any())
            {
                return NotFound("Không có bác sĩ nào.");
            }
            return Ok(doctors);
        }

        [HttpGet("InfoDoctor/{doctorId}")]
        public IActionResult GetInfoDoctorById(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem thông tin bác sĩ!");
            }
            var doctorInfo = _doctorService.GetInfoDoctorById(doctorId);
            if (doctorInfo == null)
            {
                return NotFound("Không tìm thấy thông tin bác sĩ.");
            }
            return Ok(doctorInfo);
        }

        [HttpGet("DoctorWorkSchedule/{doctorId}")]
        public IActionResult GetScheduleByDoctorId(string doctorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem lịch làm việc của bác sĩ!");
            }
            var schedule = _doctorService.GetScheduleByDoctorId(doctorId);
            if (schedule == null || !schedule.Any())
            {
                return NotFound("Không tìm thấy lịch làm việc của bác sĩ.");
            }
            return Ok(schedule);
        }

        [HttpGet("AllARVProtocol")]
        public IActionResult GetAllARVRegiemns()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không thể xem ARV Protocol!");
            }
            var arvProtocols = _doctorService.GetAllARVProtocol();
            if (arvProtocols == null || !arvProtocols.Any())
            {
                return NotFound("Không tồn tại phác đồ ARV nào.");
            }
            return Ok(arvProtocols);
        }
    }
}