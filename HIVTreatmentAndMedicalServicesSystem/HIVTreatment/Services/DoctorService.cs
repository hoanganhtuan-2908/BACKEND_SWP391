using HIVTreatment.DTOs;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository idoctorRepository;
        public DoctorService(IDoctorRepository doctorRepository)
        {
            this.idoctorRepository = doctorRepository;
        }

        public List<ARVRegimenDTO> GetAllARVRegimens()
        {
            return idoctorRepository.GetAllARVRegimens();
        }

        public List<InfoDoctorDTO> GetAllDoctors()
        {
            return idoctorRepository.GetAllDoctors();
        }
    }
}
