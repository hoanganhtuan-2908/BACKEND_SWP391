using HIVTreatment.DTOs;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository _labTestRepository;
        public LabTestService(ILabTestRepository labTestRepository)
        {
            _labTestRepository = labTestRepository;
        }
        public List<LabTestDTO> GetAllLabTests()
        {
            return _labTestRepository.GetAllLabTests().Select(l => new LabTestDTO
            {
                LabTestID = l.LabTestID,
                RequestID = l.RequestID,
                TreatmentPlantID = l.TreatmentPlantID,
                TestName = l.TestName,
                TestCode = l.TestCode,
                TestType = l.TestType,
                ResultValue = l.ResultValue,
                CD4Initial = l.CD4Initial,
                ViralLoadInitial = l.ViralLoadInitial,
                Status = l.Status,
                Description = l.Description
            }).ToList();
        }
    }
}
