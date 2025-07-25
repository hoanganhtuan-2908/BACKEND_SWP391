using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface IManagerService
    {
        (bool isSuccess, string message, string doctorId, string userId) AddDoctor(CreateDoctorDTO dto);
        List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules();
        DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId);
        (bool isSuccess, string message, string scheduleId) AddDoctorWorkSchedule(EditDoctorWorkScheduleDTO dto);
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
        bool AddARVProtocol(CreateARVProtocolDTO dto);
        bool DeleteStaff(string userId);
        List<UserDTO> GetAllManagers();
        public UserDTO GetManagerById(string userId);
        (bool isSuccess, string message, string userId) AddManager(AddManagerDTO dto);
        bool UpdateManager(string userId, UpdateManagerDTO managerDTO);
        bool DeleteManager(string userId);




    }
}
