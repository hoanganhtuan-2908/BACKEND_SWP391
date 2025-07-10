using System.Collections.Generic;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface ILabTestRepository
    {
        List<LabTest> GetAllLabTests();
    }
}