using HIVTreatment.Data;
using HIVTreatment.DTOs;
using HIVTreatment.Models;

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

        

        public Prescription GetLastPrescriptionById()
        {
            return context.Prescription.OrderByDescending(p => Convert.ToInt32(p.PrescriptionID.Substring(3)))
                                       .FirstOrDefault();
        }


        public void UpdatePrescription(Prescription prescriptionDto)
        {
            context.Update(prescriptionDto);
            context.SaveChanges();
        }
    }
}
