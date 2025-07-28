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
        bool DeleteByUserId(string userId);
        void AddDoctor(Doctor doctor);
        List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules();
        DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId);
        void AddDoctorWorkSchedule(DoctorWorkSchedule schedule);
        bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto);
        bool DeleteDoctorWorkSchedule(string scheduleId);
        void AddARVProtocol(ARVProtocol protocol);
        List<User> GetAllManagers();
        public User GetManagerById(string userId);
        public void AddUser(User user);
        int GetTotalUsers();
        Dictionary<string, int> GetUsersByRole();
        int GetTotalDoctors();
        int GetTotalPatients();
        int GetTotalLabTests();
        int GetTotalTreatmentPlans();
        int GetTotalPrescriptions();

    }
}
