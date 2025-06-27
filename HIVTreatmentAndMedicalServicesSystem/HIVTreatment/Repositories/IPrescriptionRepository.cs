using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IPrescriptionRepository
    {
        public void AddPrescription(Prescription prescription);
        public Prescription GetLastPrescriptionById();
    }
}
