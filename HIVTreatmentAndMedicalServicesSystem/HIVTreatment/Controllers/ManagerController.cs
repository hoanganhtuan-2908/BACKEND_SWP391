using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HIVTreatment.Models;
using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace HIVTreatment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "R001")]
    // R001 là RoleId của Manager
    [Authorize(Roles = "R002")]

    public class ManagerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public ManagerController(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] CreateDoctorDTO dto)
        {
            // Check if email already exists
            if (_userRepository.EmailExists(dto.Email))
            {
                return BadRequest("Email already registered.");
            }

            // 1. Sinh UserId mới
            var lastUser = _userRepository.GetLastUser();
            int nextId = 1;
            if (lastUser != null && int.TryParse(lastUser.UserId.Substring(3), out int lastId))
            {
                nextId = lastId + 1;
            }
            string newUserId = $"UID{nextId.ToString("D6")}";

            // 2. Tạo User mới
            var user = new User
            {
                UserId = newUserId,
                RoleId = "R003", // Mặc định là R003 cho Doctor
                Fullname = dto.FullName,
                Password = dto.Password,
                Email = dto.Email
            };

            try
            {
                _userRepository.Add(user); // Lưu User vào database
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to create user: {ex.Message}");
            }

            // 3. Sinh DoctorId mới (có thể theo logic tương tự UserId hoặc tùy biến)
            // Ví dụ: DT + 6 chữ số
            string newDoctorId = $"DT{nextId.ToString("D6")}"; // Có thể dùng nextId nếu muốn DoctorId và UserId tương ứng

            // 4. Tạo Doctor mới và gán UserId vừa tạo
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
                // Nếu lưu Doctor thất bại, có thể muốn rollback User đã tạo (tùy thuộc vào yêu cầu nghiệp vụ)
                // Hiện tại, tôi chỉ trả về lỗi.
                return StatusCode(500, $"Failed to add doctor: {ex.Message}");
            }

            return Ok(new { message = "Doctor added successfully.", doctorId = newDoctorId, userId = newUserId });
        }
    }
}