using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface ILabTestService
    {
        List<LabTestDTO> GetAllLabTests();
    }
}
