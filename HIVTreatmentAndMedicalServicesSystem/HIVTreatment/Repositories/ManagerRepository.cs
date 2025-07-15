using HIVTreatment.Data;
using HIVTreatment.DTOs;

namespace HIVTreatment.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly ApplicationDbContext _context;
        public ManagerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto)
        {
            var schedule = _context.DoctorWorkSchedules.FirstOrDefault(dws => dws.ScheduleID == scheduleId);
            if (schedule == null) return false;
            schedule.DoctorID = dto.DoctorID;
            schedule.SlotID = dto.SlotID;
            schedule.DateWork = dto.DateWork;
            _context.SaveChanges();
            return true;
        }
        public bool DeleteDoctorWorkSchedule(string scheduleId)
        {
            var schedule = _context.DoctorWorkSchedules.FirstOrDefault(dws => dws.ScheduleID == scheduleId);
            if (schedule == null) return false;
            _context.DoctorWorkSchedules.Remove(schedule);
            _context.SaveChanges();
            return true;
        }
    }
}
