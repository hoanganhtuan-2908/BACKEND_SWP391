using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IDoctorService
    {
        List<ARVRegimenDTO> GetAllARVRegimens();
    }
}
