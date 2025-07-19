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

        public DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId)
        {
            var schedule = (from dws in _context.DoctorWorkSchedules
                            join d in _context.Doctors on dws.DoctorID equals d.DoctorId
                            join u in _context.Users on d.UserId equals u.UserId
                            join s in _context.Slot on dws.SlotID equals s.SlotID
                            where dws.ScheduleID == scheduleId
                            select new DoctorWorkScheduleDetailDTO
                            {
                                ScheduleID = dws.ScheduleID,
                                DoctorID = d.DoctorId,
                                DoctorName = u.Fullname,
                                SlotID = s.SlotID,
                                SlotNumber = s.SlotNumber,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime,
                                DateWork = dws.DateWork
                            }).FirstOrDefault();
            return schedule;
        }

        public List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules()
        {
            var schedules = (from dws in _context.DoctorWorkSchedules
                             join d in _context.Doctors on dws.DoctorID equals d.DoctorId
                             join u in _context.Users on d.UserId equals u.UserId
                             join s in _context.Slot on dws.SlotID equals s.SlotID
                             select new DoctorWorkScheduleDetailDTO
                             {
                                 ScheduleID = dws.ScheduleID,
                                 DoctorID = d.DoctorId,
                                 DoctorName = u.Fullname,
                                 SlotID = s.SlotID,
                                 SlotNumber = s.SlotNumber,
                                 StartTime = s.StartTime,
                                 EndTime = s.EndTime,
                                 DateWork = dws.DateWork
                             }).ToList();
            return schedules;
        }
    }
}
