using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IDoctorService
    {
        List<ARVRegimenDTO> GetAllARVRegimens();
        public List<InfoDoctorDTO> GetAllDoctors();
    }
}
