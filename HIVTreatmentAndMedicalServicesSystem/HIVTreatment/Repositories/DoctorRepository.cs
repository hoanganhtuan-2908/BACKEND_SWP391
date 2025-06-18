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

        public Doctor GetByDoctorId(string doctorId)
        {
            return context.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
        }

        public Doctor GetLastDoctorId()
        {
            return context.Doctors.OrderByDescending(d => Convert.ToInt32(d.DoctorId.Substring(3))).FirstOrDefault();
        }

        public void Update(Doctor doctor)
        {
            context.Update(doctor);
            context.SaveChanges();
        }
    }
}
