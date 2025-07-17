using System.Linq;
using HIVTreatment.Data;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Repositories
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly ApplicationDbContext _context;
        public LabTestRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<LabTest> GetAllLabTests()
        {
            return _context.LabTests.ToList();
        }

        public LabTest GetLabTestById(string labTestId)
        {
            return _context.LabTests.FirstOrDefault(l => l.LabTestID == labTestId);
        }
        public void AddLabTest(LabTest labTest)
        {
            _context.LabTests.Add(labTest);
            _context.SaveChanges();
        }

        public void UpdateLabTest(LabTest labTest)
        {
            _context.LabTests.Update(labTest);
            _context.SaveChanges();
        }

        public void DeleteLabTest(string labTestId)
        {
            var labTest = _context.LabTests.FirstOrDefault(l => l.LabTestID == labTestId);
            if (labTest != null)
            {
                _context.LabTests.Remove(labTest);
                _context.SaveChanges();
            }
        }

        public List<LabTest> GetLabTestsByPatientId(string patientId)
        {
            return (from lt in _context.LabTests
                    join ba in _context.BooksAppointments on lt.RequestID equals ba.BookID
                    where ba.PatientID == patientId
                    select lt).ToList();
        }

    }
}