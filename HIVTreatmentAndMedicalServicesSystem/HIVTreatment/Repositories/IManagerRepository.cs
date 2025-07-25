using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IManagerRepository
    {
        bool DoctorExists(string doctorId);
        bool SlotExists(string slotId);
        bool ScheduleExists(string doctorId, string slotId, DateTime dateWork);
        string GetLastScheduleId();

        Doctor GetLastDoctor();
        void AddDoctor(Doctor doctor);
        List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules();
        DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId);
        void AddDoctorWorkSchedule(DoctorWorkSchedule schedule);
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
        void AddARVProtocol(ARVProtocol protocol);
        bool DeleteStaff(string userId);

    }
}
