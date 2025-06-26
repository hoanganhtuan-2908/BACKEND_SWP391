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

        public List<ARVProtocolDTO> GetAllARVProtocol()
        {
            return idoctorRepository.GetAllARVProtocol();
        }

        public List<InfoDoctorDTO> GetAllDoctors()
        {
            return idoctorRepository.GetAllDoctors();
        }

        public ARVProtocolDTO GetARVById(string ARVID)
        {
            return idoctorRepository.GetARVById(ARVID);
        }

        public InfoDoctorDTO GetInfoDoctorById(string doctorId)
        {
            return idoctorRepository.GetInfoDoctorById(doctorId);
        }

        public List<DoctorScheduleDTO> GetScheduleByDoctorId(string doctorId)
        {
            return idoctorRepository.GetScheduleByDoctorId(doctorId);
        }

        public bool updateARVProtocol(ARVProtocolDTO ARVProtocolDTO)
        {
            var ARV = idoctorRepository.GetARVById(ARVProtocolDTO.ARVID);
            if (ARV == null)
            {
                return false; // ARV regimen not found
            }

            // Map ARVRegimenDTO to ARVRegimen model
            var ARVModel = new ARVProtocol
            {
                ARVID = ARVProtocolDTO.ARVID,
                ARVCode = ARVProtocolDTO.ARVCode,
                ARVName = ARVProtocolDTO.ARVName,
                Description = ARVProtocolDTO.Description,
                AgeRange = ARVProtocolDTO.AgeRange,
                ForGroup = ARVProtocolDTO.ForGroup
            };

            idoctorRepository.updateARVProtocol(ARVModel);
            return true; // Update successful
        }
    }
}
