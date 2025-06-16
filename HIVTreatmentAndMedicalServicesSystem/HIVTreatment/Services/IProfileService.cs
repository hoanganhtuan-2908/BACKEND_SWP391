using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IProfileService
    {
        bool UpdateProfile(EditProfileUserDTO editProfileUserDTO);
    }
}
