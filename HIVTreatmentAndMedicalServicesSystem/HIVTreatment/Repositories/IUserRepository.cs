using HIVTreatment.DTOs;
using HIVTreatment.Models;

namespace HIVTreatment.Repositories
{
    public interface IUserRepository
    {
        User GetByEmail(string email);
        User GetLastUser();
        void Add(User user);
        bool EmailExists(string email);
        User GetByUserId(string UserId);
        void Update(User user);
        void UpdatePassword(string email, string newPassword);
        Doctor GetDoctorByUserId(string userId);
        public List<ARVByPatientDTO> GetARVByPatientId(string patientId);
        List<PrescriptionByPatient> GetPrescriptionByPatientId(string patientId);
        List<PrescriptionByPatient> GetPrescriptionsOfPatient(string userId);

    }
}
