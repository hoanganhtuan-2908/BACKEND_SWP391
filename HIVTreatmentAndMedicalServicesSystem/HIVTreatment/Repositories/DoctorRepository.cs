using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext context;
        public DoctorRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(Doctor doctor)
        {
            context.Add(doctor);
            context.SaveChanges();
        }

        public List<ARVRegimenDTO> GetAllARVRegimens()
        {
            var result = (from a in context.ARVRegimen
                          select new ARVRegimenDTO
                          {
                              ARVRegimenID = a.ARVRegimenID,
                              DoctorID = a.DoctorID,
                              MedicalRecordID = a.MedicalRecordID,
                              RegimenCode = a.RegimenCode,
                              ARVName = a.ARVName,
                              Description = a.Description,
                              AgeRange = a.AgeRange,
                              ForGroup = a.ForGroup

                          }).ToList();
            return result;
        }

        public List<InfoDoctorDTO> GetAllDoctors()
        {
            var result = (from d in context.Doctors
                          join u in context.Users on d.UserId equals u.UserId
                          select new InfoDoctorDTO
                          {

                              UserID = u.UserId,
                              Fullname = u.Fullname,
                              Email = u.Email,
                              Specialization = d.Specialization,
                              LicenseNumber = d.LicenseNumber,
                              ExperienceYears = d.ExperienceYears
                          }).ToList();
            return result;
        }

        public ARVRegimenDTO GetARVById(string ARVRegimenID)
        {
            var result = (from a in context.ARVRegimen
                          where a.ARVRegimenID == ARVRegimenID
                          select new ARVRegimenDTO
                          {
                              ARVRegimenID = a.ARVRegimenID,
                              DoctorID = a.DoctorID,
                              MedicalRecordID = a.MedicalRecordID,
                              RegimenCode = a.RegimenCode,
                              ARVName = a.ARVName,
                              Description = a.Description,
                              AgeRange = a.AgeRange,
                              ForGroup = a.ForGroup
                          }).FirstOrDefault();
            return result;
        }

        public Doctor GetByDoctorId(string doctorId)
        {
            return context.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
        }

        public InfoDoctorDTO GetInfoDoctorById(string DoctorID)
        {
            var result = (from d in context.Doctors
                          join u in context.Users on d.UserId equals u.UserId
                          where d.UserId == DoctorID
                          select new InfoDoctorDTO
                          {
                              UserID = u.UserId,
                              Fullname = u.Fullname,
                              Email = u.Email,
                              Specialization = d.Specialization,
                              LicenseNumber = d.LicenseNumber,
                              ExperienceYears = d.ExperienceYears
                          }).FirstOrDefault();
            return result;
        }



        public Doctor GetLastDoctorId()
        {
            return context.Doctors.OrderByDescending(d => Convert.ToInt32(d.DoctorId.Substring(3))).FirstOrDefault();
        }

        public List<DoctorScheduleDTO> GetScheduleByDoctorId(string doctorId)
        {
            var result = (from s in context.DoctorWorkSchedule
                          join slot in context.Slot on s.SlotID equals slot.SlotID
                          where s.DoctorID == doctorId
                          select new DoctorScheduleDTO
                          {
                              ScheduleID = s.ScheduleID,
                              DayOfWeek = s.DayOfWeek,
                              Status = s.Status,
                              SlotNumber = slot.SlotNumber,
                              StartTime = slot.StartTime,
                              EndTime = slot.EndTime
                          }).ToList();

            return result;
        }

        public void Update(Doctor doctor)
        {
            context.Update(doctor);
            context.SaveChanges();
        }

        public void updateARVRegimen(ARVRegimen ARVRegimen)
        {
            context.Update(ARVRegimen);
            context.SaveChanges();
        }

    }
}
