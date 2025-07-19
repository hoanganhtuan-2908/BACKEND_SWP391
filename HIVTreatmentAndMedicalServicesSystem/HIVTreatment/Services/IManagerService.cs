using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IManagerService
    {
        List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules();
        DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId);
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
    }
}
