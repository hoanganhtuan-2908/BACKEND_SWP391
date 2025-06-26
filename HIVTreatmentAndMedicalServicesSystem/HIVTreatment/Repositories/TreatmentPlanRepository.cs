using HIVTreatment.Data;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.EntityFrameworkCore;

public class TreatmentPlanRepository : ITreatmentPlanRepository
{
    private readonly ApplicationDbContext _context;

    public TreatmentPlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<TreatmentPlan> GetAll()
    {
        return _context.TreatmentPlans
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }

    public List<TreatmentPlan> GetByPatient(string patientId)
    {
        return _context.TreatmentPlans
            .Where(p => p.PatientID == patientId)
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }

    public List<TreatmentPlan> GetByDoctorUserId(string userId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == userId);
        if (doctor == null) return new List<TreatmentPlan>();

        return _context.TreatmentPlans
            .Where(p => p.DoctorID == doctor.DoctorId)
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }
    public List<TreatmentPlan> GetByPatientAndDoctor(string patientId, string doctorUserId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == doctorUserId);
        if (doctor == null) return new List<TreatmentPlan>();

        return _context.TreatmentPlans
            .Where(p => p.PatientID == patientId && p.DoctorID == doctor.DoctorId)
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }

}
