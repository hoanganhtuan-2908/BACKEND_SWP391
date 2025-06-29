using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface IPrescriptionService
    {
        public bool AddPrescription(PrescriptionDTO prescriptionDto);
        public bool UpdatePrescription(UpdatePrescriptionDTO updatePrescriptionDTO);
        public List<Prescription> GetAllPrescription();
        public Prescription GetPrescriptionById(string prescriptionId);
        public List<Prescription> GetPrescriptionByPatientAndDoctor(string medicalRecordId, string doctorId);
    }
}
