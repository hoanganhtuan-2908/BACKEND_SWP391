using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IProfileService
    {
        bool UpdateProfile(EditProfileUserDTO editProfileUserDTO);

        bool UpdateDoctorProfile(EditprofileDoctorDTO editProfileDoctorDTO);
        List<PatientDTO> GetAllPatient();
    }
}
