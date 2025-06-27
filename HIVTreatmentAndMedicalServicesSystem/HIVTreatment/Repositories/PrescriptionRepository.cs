using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext context;
        public PrescriptionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void AddPrescription(Prescription prescription)
        {
            context.Add(prescription);
            context.SaveChanges();
        }

        public List<Prescription> GetAllPrescription()
        {
            var result = (from p in context.Prescription select new Prescription
            {
                PrescriptionID = p.PrescriptionID,
                MedicalRecordID = p.MedicalRecordID,
                MedicationID = p.MedicationID,
                DoctorID = p.DoctorID,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Dosage = p.Dosage,
                LineOfTreatment = p.LineOfTreatment
            }).ToList();
            return result;
        }

        public Prescription GetLastPrescriptionById()
        {
            return context.Prescription.OrderByDescending(p => Convert.ToInt32(p.PrescriptionID.Substring(3)))
                                       .FirstOrDefault();
        }

        public Prescription GetPrescriptionById(string prescriptionId)
        {
            var result = (from p in context.Prescription where p.PrescriptionID == prescriptionId select new Prescription
            {
                PrescriptionID = p.PrescriptionID,
                MedicalRecordID = p.MedicalRecordID,
                MedicationID = p.MedicationID,
                DoctorID = p.DoctorID,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Dosage = p.Dosage,
                LineOfTreatment = p.LineOfTreatment
            }).FirstOrDefault();
            return result;
        }

        public List<Prescription> GetPrescriptionByPatientAndDoctor(string medicalRecordId, string doctorId)
        {
            return context.Prescription.
                Where(p => p.MedicalRecordID == medicalRecordId && p.DoctorID == doctorId)
                .Include(p => p.DoctorID).
                Include(p => p.MedicalRecordID)
                .ToList();
        }

        public void UpdatePrescription(Prescription prescriptionDto)
        {
            context.Update(prescriptionDto);
            context.SaveChanges();
        }
    }
}
