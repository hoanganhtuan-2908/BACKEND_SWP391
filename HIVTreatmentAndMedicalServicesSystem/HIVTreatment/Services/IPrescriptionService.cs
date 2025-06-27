using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface IPrescriptionService
    {
        public bool AddPrescription(PrescriptionDTO prescriptionDto);
        public bool UpdatePrescription(UpdatePrescriptionDTO updatePrescriptionDTO);
    }
}
