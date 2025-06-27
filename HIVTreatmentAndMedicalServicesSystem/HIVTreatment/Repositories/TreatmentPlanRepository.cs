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
        return _context.TreatmentPlan
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }

    public List<TreatmentPlan> GetByPatient(string patientId)
    {
        return _context.TreatmentPlan
            .Where(p => p.PatientID == patientId)
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }

    public List<TreatmentPlan> GetByDoctorUserId(string userId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == userId);
        if (doctor == null) return new List<TreatmentPlan>();

        return _context.TreatmentPlan
            .Where(p => p.DoctorID == doctor.DoctorId)
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }
    public List<TreatmentPlan> GetByPatientAndDoctor(string patientId, string doctorUserId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == doctorUserId);
        if (doctor == null) return new List<TreatmentPlan>();

        return _context.TreatmentPlan
            .Where(p => p.PatientID == patientId && p.DoctorID == doctor.DoctorId)
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .ToList();
    }

    public void AddTreatmentPlan(TreatmentPlan treatmentPlan)
    {
        _context.TreatmentPlan.Add(treatmentPlan);
        _context.SaveChanges();
    }
    public TreatmentPlan GetLastTreatmentPlantId()
    {
        return _context.TreatmentPlan.OrderByDescending(t => Convert.ToInt32(t.TreatmentPlanID.Substring(3))).FirstOrDefault();
    }

    public void UpdateTreatmentPlan(TreatmentPlan treatmentPlan)
    {
        _context.TreatmentPlan
            .Update(treatmentPlan);
        _context.SaveChanges();
    }
}
