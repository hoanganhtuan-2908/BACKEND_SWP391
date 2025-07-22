using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class ManagerService : IManagerService
    {

        
        private readonly IManagerRepository _managerRepository;
        private readonly IUserRepository _userRepository;
        public ManagerService(IManagerRepository managerRepository, IUserRepository userRepository)
        {
            _managerRepository = managerRepository;
            _userRepository = userRepository;
        }
        public (bool isSuccess, string message, string doctorId, string userId) AddDoctor(CreateDoctorDTO dto)
        {
            // 1. Kiểm tra email đã tồn tại
            if (_userRepository.EmailExists(dto.Email))
            {
                return (false, "Email này đã tồn tại!", null, null);
            }

            // 2. Tạo UserId
            var lastUser = _userRepository.GetLastUser();
            int nextUserId = 1;
            if (lastUser != null && int.TryParse(lastUser.UserId.Substring(2), out int lastId))
            {
                nextUserId = lastId + 1;
            }
            string newUserId = $"UI{nextUserId:D6}";

            // 3. Tạo DoctorId: DT000001
            var lastDoctor = _managerRepository.GetLastDoctor();
            int nextDoctorId = 1;
            if (lastDoctor != null && int.TryParse(lastDoctor.DoctorId.Substring(2), out int lastDId))
            {
                nextDoctorId = lastDId + 1;
            }
            string newDoctorId = $"DT{nextDoctorId:D6}";

            // 4. Tạo user mới
            var user = new User
            {
                UserId = newUserId,
                Fullname = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                RoleId = "R003", // Role bác sĩ
                Address = dto.Address,
                Image = "Doctor.png"
            };
            _userRepository.Add(user);

            // 5. Tạo doctor mới
            var doctor = new Doctor
            {
                DoctorId = newDoctorId,
                UserId = newUserId,
                Specialization = dto.Specialization,
                LicenseNumber = dto.LicenseNumber,
                ExperienceYears = dto.ExperienceYears
            };
            _managerRepository.AddDoctor(doctor);

            return (true, "Thêm mới Doctor thành công.", newDoctorId, newUserId);
        }
        
        public List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules()
        {
            return _managerRepository.GetAllDoctorWorkSchedules();
        }
        public DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId)
        {
            return _managerRepository.GetDoctorWorkScheduleDetail(scheduleId);
        }

        public bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto)
            => _managerRepository.UpdateDoctorWorkSchedule(scheduleId, dto);
        public bool DeleteDoctorWorkSchedule(string scheduleId)
        => _managerRepository.DeleteDoctorWorkSchedule(scheduleId);

        
    }
}
