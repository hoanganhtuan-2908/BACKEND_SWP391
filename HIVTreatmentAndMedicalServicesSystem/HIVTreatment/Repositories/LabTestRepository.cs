using System.Linq;
using HIVTreatment.Data;
using HIVTreatment.Models;

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
    }
}