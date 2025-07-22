using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IManagerRepository
    {
        Doctor GetLastDoctor();
        void AddDoctor(Doctor doctor);
        List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules();
        DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId);
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
    }
}
