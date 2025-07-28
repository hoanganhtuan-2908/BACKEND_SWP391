using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class ManagerService : IManagerService
    {

        
        private readonly IManagerRepository _managerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        public ManagerService(IManagerRepository managerRepository, IUserRepository userRepository, IDoctorRepository doctorRepository)
        {
            _managerRepository = managerRepository;
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
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
        public (bool isSuccess, string message, string scheduleId) AddDoctorWorkSchedule(EditDoctorWorkScheduleDTO dto)
        {
            // 1. Kiểm tra Doctor tồn tại
            if (!_managerRepository.DoctorExists(dto.DoctorID))
            {
                return (false, "Doctor không tồn tại.", null);
            }

            // 2. Kiểm tra Slot tồn tại
            if (!_managerRepository.SlotExists(dto.SlotID))
            {
                return (false, "Slot không tồn tại.", null);
            }

            // 3. Kiểm tra lịch đã tồn tại chưa (tránh trùng)
            if (_managerRepository.ScheduleExists(dto.DoctorID, dto.SlotID, dto.DateWork))
            {
                return (false, "Lịch làm việc này đã tồn tại.", null);
            }

            // 4. Tạo ScheduleID mới: DW000001
            string lastId = _managerRepository.GetLastScheduleId();
            int nextId = 1;
            if (!string.IsNullOrEmpty(lastId) && int.TryParse(lastId.Substring(2), out int parsedId))
            {
                nextId = parsedId + 1;
            }
            string newScheduleId = $"DW{nextId:D6}";

            // 5. Tạo đối tượng và lưu vào DB
            var schedule = new DoctorWorkSchedule
            {
                ScheduleID = newScheduleId,
                DoctorID = dto.DoctorID,
                SlotID = dto.SlotID,
                DateWork = dto.DateWork
            };

            _managerRepository.AddDoctorWorkSchedule(schedule);
            return (true, "Thêm lịch làm việc thành công.", newScheduleId);
        }

        public bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto)
            => _managerRepository.UpdateDoctorWorkSchedule(scheduleId, dto);
        public bool DeleteDoctorWorkSchedule(string scheduleId)
        => _managerRepository.DeleteDoctorWorkSchedule(scheduleId);

        public bool AddARVProtocol(CreateARVProtocolDTO dto)
        {
            // Kiểm tra trùng ARVCode hoặc ARVName
            var allProtocols = _doctorRepository.GetAllARVProtocol();
            if (allProtocols.Any(a => a.ARVCode == dto.ARVCode || a.ARVName == dto.ARVName))
                return false;

            // Sinh ARVID mới
            var last = allProtocols.OrderByDescending(a => a.ARVID).FirstOrDefault();
            int nextId = 1;
            if (last != null && int.TryParse(last.ARVID.Substring(2), out int lastId))
                nextId = lastId + 1;
            string newARVID = $"AP{nextId:D6}";

            var protocol = new ARVProtocol
            {
                ARVID = newARVID,
                ARVCode = dto.ARVCode,
                ARVName = dto.ARVName,
                Description = dto.Description,
                AgeRange = dto.AgeRange,
                ForGroup = dto.ForGroup
            };

            _managerRepository.AddARVProtocol(protocol);
            return true;
        }

        public bool DeleteStaff(string userId)
        {
            var user = _userRepository.GetByUserId(userId);

            // Kiểm tra tk Staff
            if (user == null || user.RoleId != "R004")
                return false;

            return _managerRepository.DeleteByUserId(userId);
        }


        public List<UserDTO> GetAllManagers()
        {
            var managers = _managerRepository.GetAllManagers();
            return managers.Select(u => new UserDTO
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                Fullname = u.Fullname,
                Password = u.Password,
                Email = u.Email,
                Address = u.Address
            }).ToList();
        }

        public UserDTO GetManagerById(string userId)
        {
            var manager = _managerRepository.GetManagerById(userId);
            if (manager == null) return null;

            return new UserDTO
            {
                UserId = manager.UserId,
                RoleId = manager.RoleId,
                Fullname = manager.Fullname,
                Password = manager.Password,
                Email = manager.Email,
                Address = manager.Address
            };
        }

       public (bool isSuccess, string message, string userId) AddManager(AddManagerDTO dto)
        {
            if (_userRepository.EmailExists(dto.Email))
            {
                return (false, "Email này đã tồn tại trong hệ thống!", null);
            }

            var lastUser = _userRepository.GetLastUser();
            int nextUserId = 1;
            if (lastUser != null && int.TryParse(lastUser.UserId.Substring(2), out int lastId))
            {
                nextUserId = lastId + 1;
            }
            string newUserId = $"UI{nextUserId:D6}";

            var newManager = new User
            {
                UserId = newUserId,
                RoleId = "R002",
                Fullname = dto.Fullname,
                Password = dto.Password,
                Email = dto.Email,
                Address = dto.Address
            };

            _managerRepository.AddUser(newManager);

            return (true, "Thêm mới Manager thành công!", newUserId);
        }

        public bool UpdateManager(string userId, UpdateManagerDTO managerDTO)
        {
            var user = _userRepository.GetByUserId(userId);
            if (user == null || user.RoleId != "R002")
                return false; // Chỉ cho phép sửa manager

            // Kiểm tra trùng email (nếu đổi email)
            if (user.Email != managerDTO.Email && _userRepository.EmailExists(managerDTO.Email))
                return false;

            user.Fullname = managerDTO.Fullname;
            user.Email = managerDTO.Email;
            user.Address = managerDTO.Address;
            user.Image = string.IsNullOrEmpty(managerDTO.Image) ? "manager.png" : managerDTO.Image;

            _userRepository.Update(user);
            return true;
        }

        public bool DeleteManager(string userId)
        {
            var user = _userRepository.GetByUserId(userId);
            if (user == null || user.RoleId != "R002")  // Chỉ xóa nếu là Manager
                return false;
            return _managerRepository.DeleteByUserId(userId);
        }

        public int GetTotalUsers()
        {
            return _managerRepository.GetTotalUsers();
        }

        public Dictionary<string, int> GetUsersByRole()
        {
            return _managerRepository.GetUsersByRole();
        }

        public int GetTotalDoctors()
        {
            return _managerRepository.GetTotalDoctors();
        }

        public int GetTotalPatients()
        {
            return _managerRepository.GetTotalPatients();
        }

        public int GetTotalLabTests()
        {
            return _managerRepository.GetTotalLabTests();
        }

        public int GetTotalTreatmentPlans()
        {
            return _managerRepository.GetTotalTreatmentPlans();
        }

        public int GetTotalPrescriptions()
        {
            return _managerRepository.GetTotalPrescriptions();
        }

        public ManagerDashboardDTO GetDashboardStatistics()
        {
            return new ManagerDashboardDTO
            {
                TotalUsers = _managerRepository.GetTotalUsers(),
                UsersByRole = _managerRepository.GetUsersByRole(),
                TotalDoctors = _managerRepository.GetTotalDoctors(),
                TotalPatients = _managerRepository.GetTotalPatients(),
                TotalLabTests = _managerRepository.GetTotalLabTests(),
                TotalTreatmentPlans = _managerRepository.GetTotalTreatmentPlans(),
                TotalPrescriptions = _managerRepository.GetTotalPrescriptions()
            };
        }
    }
}
