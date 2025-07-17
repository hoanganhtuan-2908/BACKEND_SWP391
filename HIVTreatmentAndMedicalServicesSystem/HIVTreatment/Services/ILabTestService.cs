using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Services
{
    public interface ILabTestService
    {
        List<LabTestDTO> GetAllLabTests();
        LabTestDTO GetLabTestById(string labTestId);
        List<LabTest> GetLabTestsByPatientId(string patientId);
        void CreateLabTest(CreateLabTestDTO dto);
        public bool UpdateLabTest(string labTestId, UpdateLabTestDTO dto);
        bool DeleteLabTest(string labTestId);
    }
}
