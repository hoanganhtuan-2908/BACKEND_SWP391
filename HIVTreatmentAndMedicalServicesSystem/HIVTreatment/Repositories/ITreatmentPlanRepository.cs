using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface ITreatmentPlanRepository
    {
        List<TreatmentPlan> GetAll();
        List<TreatmentPlan> GetByPatient(string patientId);
        List<TreatmentPlan> GetByDoctorUserId(string userId);
        List<TreatmentPlan> GetByPatientAndDoctor(string patientId, string doctorUserId);

    }
}
