using HIVTreatment.Data;
using HIVTreatment.DTOs;
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
        .Select(tp => new TreatmentPlan
        {
            TreatmentPlanID = tp.TreatmentPlanID,
            PatientID = tp.PatientID,
            DoctorID = tp.DoctorID,
            ARVProtocol = tp.ARVProtocol,
            TreatmentLine = tp.TreatmentLine,
            Diagnosis = tp.Diagnosis,
            TreatmentResult = tp.TreatmentResult
        })
        .ToList();
    }

    public List<TreatmentPlan> GetByPatient(string patientId)
    {
        return _context.TreatmentPlan.Where(tp => tp.PatientID == patientId)
            .Select(tp => new TreatmentPlan
            {
                TreatmentPlanID = tp.TreatmentPlanID,
                PatientID = tp.PatientID,
                DoctorID = tp.DoctorID,
                ARVProtocol = tp.ARVProtocol,
                TreatmentLine = tp.TreatmentLine,
                Diagnosis = tp.Diagnosis,
                TreatmentResult = tp.TreatmentResult
            })
            .ToList();
    }

    public List<TreatmentPlan> GetByDoctorUserId(string userId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == userId);
        if (doctor == null) return new List<TreatmentPlan>();

        return _context.TreatmentPlan
            .Where(tp => tp.DoctorID == doctor.DoctorId)
            .Include(tp => tp.Patient)
                .ThenInclude(p => p.User)
            .ToList();

    }
    public List<TreatmentPlan> GetByPatientAndDoctor(string patientId, string doctorUserId)
    {
        var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == doctorUserId);
        if (doctor == null) return new List<TreatmentPlan>();
        return _context.TreatmentPlan
    .Where(tp => tp.PatientID == patientId && tp.DoctorID == doctor.DoctorId)
    .Include(tp => tp.Patient)
        .ThenInclude(p => p.User)
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

    public UpdateTreatmentPlanDTO GetTreatmentPlanById(string treatmentPlanId)
    {
        var result = (from t in _context.TreatmentPlan
                      where t.TreatmentPlanID ==treatmentPlanId
                      select new UpdateTreatmentPlanDTO
                      {
                          TreatmentPlanID = t.TreatmentPlanID,
                          PatientID = t.PatientID,
                          DoctorID = t.DoctorID,
                          ARVProtocol = t.ARVProtocol,
                          TreatmentLine = t.TreatmentLine,
                          Diagnosis = t.Diagnosis,
                          TreatmentResult = t.TreatmentResult
                      }).FirstOrDefault();
        return result;
    }
}
