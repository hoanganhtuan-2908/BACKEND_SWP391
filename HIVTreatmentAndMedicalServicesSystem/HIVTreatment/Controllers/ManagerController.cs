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
        private readonly IUserService _userService;
        private readonly IManagerService _managerService;

        public ManagerController(ApplicationDbContext context, IUserRepository userRepository, IDoctorService doctorService,IUserService userService, IManagerService managerService)
        {
            _context = context;
            _userRepository = userRepository;
            _doctorService = doctorService;
            _userService = userService;
            _managerService = managerService;
        }

        [HttpGet("AllUsers")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            if (users == null || !users.Any())
            {
                return NotFound("Không có người dùng nào.");
            }
            return Ok(users);
        }

        [HttpGet("User/{userId}")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetUserById(string userId)
        {
            var user = _userService.GetByUserId(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }
            return Ok(user);
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
        [HttpPost("AddDoctor")]
        public IActionResult AddDoctor([FromBody] CreateDoctorDTO dto)
        {
            try
            {
                var result = _managerService.AddDoctor(dto);

                if (!result.isSuccess)
                {
                    return BadRequest(new { message = result.message });
                }

                return Ok(new
                {
                    message = result.message,
                    doctorId = result.doctorId,
                    userId = result.userId
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có logging
                return StatusCode(500, new { message = "Server error: " + ex.Message });
            }
        }

        

        [HttpPut("UpdateDoctor/{doctorId}")]
        public async Task<IActionResult> EditDoctor(string doctorId, [FromBody] EditDoctorDTO dto)
        {
            // 1. Tìm doctor theo doctorId
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor == null)
                return NotFound("Không tìm thấy bác sĩ.");

            // 2. Tìm user theo UserId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == doctor.UserId);
            if (user == null)
                return NotFound("Không tìm thấy tài khoản bác sĩ.");

            // 3. Kiểm tra trùng email (nếu email thay đổi)
            if (user.Email != dto.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != user.UserId);
                if (emailExists)
                    return BadRequest("Email đã tồn tại.");
            }

            // 4. Kiểm tra trùng LicenseNumber (nếu thay đổi)
            if (doctor.LicenseNumber != dto.LicenseNumber)
            {
                var licenseExists = await _context.Doctors.AnyAsync(d => d.LicenseNumber == dto.LicenseNumber && d.DoctorId != doctorId);
                if (licenseExists)
                    return BadRequest("Số giấy phép đã tồn tại.");
            }

            // 5. Cập nhật thông tin
            user.Fullname = dto.FullName;
            user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.Image)) user.Image = dto.Image;

            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.ExperienceYears = dto.ExperienceYears;

            // 6. Lưu thay đổi
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thông tin bác sĩ thành công." });
        }

        [HttpGet("AllDoctorWorkSchedules")]
        public IActionResult GetAllDoctorWorkSchedules()
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem lịch làm việc của bác sĩ!");
            }

            var schedules = _managerService.GetAllDoctorWorkSchedules();

            if (schedules == null || !schedules.Any())
            {
                return NotFound("Không có lịch làm việc nào.");
            }
            return Ok(schedules);
        }


        [HttpGet("DoctorWorkScheduleDetail/{scheduleId}")]
        public IActionResult GetDoctorWorkScheduleDetail(string scheduleId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem chi tiết lịch làm việc!");
            }

            var schedule = _managerService.GetDoctorWorkScheduleDetail(scheduleId);

            if (schedule == null)
            {
                return NotFound("Không tìm thấy lịch làm việc này.");
            }
            return Ok(schedule);
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
        
        [HttpPost("AddDoctorWorkSchedule")]
        public IActionResult AddDoctorWorkSchedule([FromBody] EditDoctorWorkScheduleDTO dto)
        {
            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
                if (!allowedRoles.Contains(userRole))
                {
                    return Forbid("Bạn không có quyền thêm lịch làm việc!");
                }

                var result = _managerService.AddDoctorWorkSchedule(dto);

                if (!result.isSuccess)
                {
                    return BadRequest(result.message);
                }

                return Ok(new
                {
                    message = result.message,
                    scheduleId = result.scheduleId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã có lỗi xảy ra trong hệ thống. Vui lòng thử lại sau.");
            }
        }

        [HttpPut("UpdateDoctorWorkSchedule/{scheduleId}")]
        public IActionResult EditDoctorWorkSchedule(string scheduleId, [FromBody] EditDoctorWorkScheduleDTO dto)
        {
            try
            {
                var result = _managerService.UpdateDoctorWorkSchedule(scheduleId, dto);
                if (!result)
                    return NotFound("Không tìm thấy lịch làm việc này hoặc cập nhật không thành công.");

                return Ok(new { message = "Cập nhật lịch làm việc thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }

        


        [HttpDelete("DeleteDoctorWorkSchedule/{scheduleId}")]
        public IActionResult DeleteDoctorWorkSchedule(string scheduleId)
        {
            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var allowedRoles = new[] { "R001", "R002" }; // Admin, Manager
                if (!allowedRoles.Contains(userRole))
                {
                    return Forbid("Bạn không có quyền xóa lịch làm việc!");
                }

                var result = _managerService.DeleteDoctorWorkSchedule(scheduleId);
                if (!result)
                    return NotFound("Không tìm thấy lịch làm việc để xóa.");

                return Ok(new { message = "Xóa lịch làm việc thành công." });
            }
            catch (Exception)
            {
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình xóa lịch làm việc.");
            }
        }

        [HttpGet("AllARVProtocol")]
        public IActionResult GetAllARVRegiemns()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem ARV Protocol!");
            }
            var arvProtocols = _doctorService.GetAllARVProtocol();
            if (arvProtocols == null || !arvProtocols.Any())
            {
                return NotFound("Không có phác đồ ARV nào.");
            }
            return Ok(arvProtocols);
        }

        [HttpGet("ARVProtocol/{ARVProtocolID}")]
        public IActionResult GetARVById(string ARVProtocolID)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001", "R002","R003", "R005" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền xem phác đồ ARV!");
            }
            var arvProtocol = _doctorService.GetARVById(ARVProtocolID);
            if (arvProtocol == null)
            {
                return NotFound("Không tìm thấy phác đồ ARV.");
            }
            return Ok(arvProtocol);
        }

        [HttpPost("AddARVProtocol")]
        [Authorize(Roles = "R001,R002")]
        public IActionResult AddARVProtocol([FromBody] CreateARVProtocolDTO dto)
        {
            try
            {
                var result = _managerService.AddARVProtocol(dto);
                if (!result)
                    return BadRequest("Phác đồ ARV đã tồn tại hoặc dữ liệu không hợp lệ.");

                return Ok("Thêm phác đồ ARV thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi khi thêm phác đồ ARV. Chi tiết: {ex.Message}");
            }
        }

        [HttpPut("UpdateARVProtocol")]
        public IActionResult updateARVRegimen(ARVProtocolDTO ARVProtocolDTO)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var allowedRoles = new[] { "R001","R002", "R003" };
            if (!allowedRoles.Contains(userRole))
            {
                return Forbid("Bạn không có quyền sửa phác đồ ARV!");
            }
            var arvUpdated = _doctorService.updateARVProtocol(ARVProtocolDTO);
            if (!arvUpdated)
            {
                return NotFound("Phác đồ ARV không tồn tại hoặc cập nhật không thành công.");
            }
            return Ok("Cập nhật phác đồ ARV thành công.");
        }

        [HttpGet("AllStaff")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetAllStaff()
        {
            var staffList = _userService.GetAllStaff();
            if (staffList == null || !staffList.Any())
            {
                return NotFound("Không có staff nào.");
            }
            return Ok(staffList);
        }
        [HttpGet("Staff/{userId}")]
        [Authorize(Roles = "R001,R002")] // Admin, Manager
        public IActionResult GetStaffById(string userId)
        {
            var staff = _userService.GetStaffById(userId);
            if (staff == null)
                return NotFound("Không tìm thấy staff.");
            return Ok(staff);
        }

        [HttpPost("AddStaff")]
        [Authorize(Roles = "R001,R002")]
        public IActionResult AddStaff([FromBody] CreateStaffDTO staffDTO)
        {
            var user = _userService.AddStaff(staffDTO);
            if (user == null)
                return BadRequest("Email đã tồn tại hoặc dữ liệu không hợp lệ.");
            return Ok(new { message = "Thêm staff thành công.", userId = user.UserId });
        }

        [HttpPut("UpdateStaff/{userId}")]
        public IActionResult UpdateStaff(string userId, [FromBody] UpdateStaffDTO staffDTO)
        {
            var result = _userService.UpdateStaff(userId, staffDTO);
            if (!result)
                return BadRequest("Cập nhật staff thất bại (không tìm thấy staff hoặc email đã tồn tại).");
            return Ok("Cập nhật staff thành công.");
        }

        [HttpDelete("DeleteStaff/{userId}")]
        [Authorize(Roles = "R001,R002")]
        public IActionResult DeleteStaff(string userId)
        {
            try
            {
                var success = _managerService.DeleteStaff(userId);
                if (!success)
                    return NotFound(new { message = "Xóa Staff thất bại!" });

                return Ok(new { message = "Xóa Staff thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa Staff.", detail = ex.Message });
            }
        }

        
        [HttpGet("GetAllManagers")]
        [Authorize(Roles = "R001")]
        public IActionResult GetAllManagers()
        {
            var managers = _managerService.GetAllManagers();
            return Ok(managers);
        }

        [HttpGet("GetManagerById/{userId}")]
        [Authorize(Roles = "R001")]
        public IActionResult GetManagerById(string userId)
        {
            var manager = _managerService.GetManagerById(userId);

            if (manager == null)
            {
                return NotFound("Không tìm thấy Manager.");
            }

            return Ok(manager);
        }

        [HttpPost("AddManager")]
        [Authorize(Roles = "R001")]
        public IActionResult AddManager([FromBody] AddManagerDTO dto)
        {
            try
            {
                var result = _managerService.AddManager(dto);
                if (!result.isSuccess)
                {
                    return BadRequest(new { message = result.message });
                }

                return Ok(new { message = result.message, userId = result.userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi!", detail = ex.Message });
            }
        }

        [HttpPut("UpdateManager/{userId}")]
        [Authorize(Roles = "R001")]
        public IActionResult UpdateManager(string userId, [FromBody] UpdateManagerDTO dto)
        {
            try
            {
                var result = _managerService.UpdateManager(userId, dto);
                if (!result)
                {
                    return BadRequest(new
                    {
                        message = "Cập nhật thất bại. Không tìm thấy Manager hoặc email đã tồn tại."
                    });
                }

                return Ok(new
                {
                    message = "Cập nhật tài khoản Manager thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi trong quá trình xử lý.",
                    detail = ex.Message
                });
            }
        }

        [HttpDelete("DeleteManager/{userId}")]
        [Authorize(Roles = "R001")]
        public IActionResult DeleteManager(string userId)
        {
            try
            {
                var result = _managerService.DeleteManager(userId);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy Manager hoặc không có quyền xóa." });

                return Ok(new { message = "Xóa Manager thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa.", detail = ex.Message });
            }
        }


    }


}