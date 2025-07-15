using HIVTreatment.DTOs;

namespace HIVTreatment.Repositories
{
    public interface IManagerRepository
    {
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
    }
}
