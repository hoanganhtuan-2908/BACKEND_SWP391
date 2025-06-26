using HIVTreatment.Data;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public class TreatmentPlanRepository : ITreatmentPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public TreatmentPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TreatmentPlan> GetAll()
        {
            return _context.TreatmentPlans.ToList();
        }

        public List<TreatmentPlan> GetByPatient(string patientId)
        {
            return _context.TreatmentPlans
                .Where(p => p.PatientID == patientId)
                .ToList();
        }
    }
}
