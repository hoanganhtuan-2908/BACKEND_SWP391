using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IPrescriptionService
    {
        public bool AddPrescription(PrescriptionDTO prescriptionDto);
    }
}
