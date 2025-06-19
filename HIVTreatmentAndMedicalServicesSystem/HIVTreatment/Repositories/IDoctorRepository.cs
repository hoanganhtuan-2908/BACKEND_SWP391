using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IDoctorRepository
    {
        Doctor GetByDoctorId(string doctorId);

        Doctor GetLastDoctorId();

        void Add(Doctor doctor);

        void Update(Doctor doctor);
        List<ARVRegimenDTO> GetAllARVRegimens();
        List<InfoDoctorDTO> GetAllDoctors();
        InfoDoctorDTO GetInfoDoctorById(string patientId);

        ARVRegimenDTO GetARVById(string ARVRegimenID);

        void updateARVRegimen(ARVRegimen ARVRegimen);
    }
}
