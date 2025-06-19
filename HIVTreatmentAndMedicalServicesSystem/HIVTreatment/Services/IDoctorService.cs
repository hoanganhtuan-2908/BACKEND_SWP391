using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IDoctorService
    {
        List<ARVRegimenDTO> GetAllARVRegimens();
        public List<InfoDoctorDTO> GetAllDoctors();

        InfoDoctorDTO GetInfoDoctorById(string doctorId);
        ARVRegimenDTO GetARVById(string ARVRegimenID);
        bool updateARVRegimen(ARVRegimenDTO ARVRegimenDTO);
    }
}
