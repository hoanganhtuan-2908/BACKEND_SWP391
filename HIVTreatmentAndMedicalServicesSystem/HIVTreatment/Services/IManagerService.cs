using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IManagerService
    {
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
    }
}
