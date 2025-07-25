using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly ApplicationDbContext _context;
        public ManagerRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public bool DoctorExists(string doctorId)
        {
            return _context.Doctors.Any(d => d.DoctorId == doctorId);
        }

        public bool SlotExists(string slotId)
        {
            return _context.Slot.Any(s => s.SlotID == slotId);
        }

        public bool ScheduleExists(string doctorId, string slotId, DateTime dateWork)
        {
            return _context.DoctorWorkSchedules.Any(s =>
                s.DoctorID == doctorId &&
                s.SlotID == slotId &&
                s.DateWork.Date == dateWork.Date);
        }

        public string GetLastScheduleId()
        {
            return _context.DoctorWorkSchedules
                .OrderByDescending(s => s.ScheduleID)
                .Select(s => s.ScheduleID)
                .FirstOrDefault();
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

        public Doctor GetLastDoctor()
        {
            return _context.Doctors.OrderByDescending(d => d.DoctorId).FirstOrDefault();
        }

        public void AddDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }

        public void AddDoctorWorkSchedule(DoctorWorkSchedule schedule)
        {
            _context.DoctorWorkSchedules.Add(schedule);
            _context.SaveChanges();
        }

        public void AddARVProtocol(ARVProtocol protocol)
        {
            _context.ARVProtocol.Add(protocol);
            _context.SaveChanges();
        }

        public bool DeleteStaff(string userId)
        {
            // Tìm user với RoleId = R003 (Staff)
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId && u.RoleId == "R004");
            if (user == null)
                return false;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
    }
}
