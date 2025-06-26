using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface ITreatmentPlanRepository
    {
        List<TreatmentPlan> GetAll();
        List<TreatmentPlan> GetByPatient(string patientId);
    }
}
