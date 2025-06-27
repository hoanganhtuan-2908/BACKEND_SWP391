using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        public PrescriptionService(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        public bool AddPrescription(PrescriptionDTO prescriptionDto)
        {
            var lastPrescription = _prescriptionRepository.GetLastPrescriptionById();
            int nextId = 1;
            if (lastPrescription != null && lastPrescription.PrescriptionID?.Length >= 8)
            {
                string startNumber = lastPrescription.PrescriptionID.Substring(2);
                if (int.TryParse(startNumber, out int parsed))
                {
                    nextId = parsed + 1;
                }
            }
            string newPrescriptionID = "PR" + nextId.ToString("D6");
            var newPrescription = new Prescription
            {
                PrescriptionID = newPrescriptionID,
                MedicalRecordID = prescriptionDto.MedicalRecordID,
                MedicationID = prescriptionDto.MedicationID,
                DoctorID = prescriptionDto.DoctorID,
                StartDate = prescriptionDto.StartDate,
                EndDate = prescriptionDto.EndDate,
                Dosage = prescriptionDto.Dosage,
                LineOfTreatment = prescriptionDto.LineOfTreatment
            };
            _prescriptionRepository.AddPrescription(newPrescription); 
            return true;
        }
    }
}
