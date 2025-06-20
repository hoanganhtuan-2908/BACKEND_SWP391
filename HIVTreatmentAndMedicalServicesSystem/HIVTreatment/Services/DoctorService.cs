using HIVTreatment.DTOs;
using HIVTreatment.Models;
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

        public ARVRegimenDTO GetARVById(string ARVRegimenID)
        {
            return idoctorRepository.GetARVById(ARVRegimenID);
        }

        public InfoDoctorDTO GetInfoDoctorById(string doctorId)
        {
            return idoctorRepository.GetInfoDoctorById(doctorId);
        }

        public bool updateARVRegimen(ARVRegimenDTO ARVRegimenDTO)
        {
            var ARV = idoctorRepository.GetARVById(ARVRegimenDTO.ARVRegimenID);
            if (ARV == null)
            {
                return false; // ARV regimen not found
            }

            // Map ARVRegimenDTO to ARVRegimen model
            var ARVModel = new ARVRegimen
            {
                ARVRegimenID = ARVRegimenDTO.ARVRegimenID,
                DoctorID = ARVRegimenDTO.DoctorID,
                MedicalRecordID = ARVRegimenDTO.MedicalRecordID,
                RegimenCode = ARVRegimenDTO.RegimenCode,
                ARVName = ARVRegimenDTO.ARVName,
                Description = ARVRegimenDTO.Description,
                AgeRange = ARVRegimenDTO.AgeRange,
                ForGroup = ARVRegimenDTO.ForGroup
            };

            idoctorRepository.updateARVRegimen(ARVModel);
            return true; // Update successful
        }
    }
}
