using HIVTreatment.Models;
using HIVTreatment.DTOs;

namespace HIVTreatment.Services
{
    public interface IUserService
    {
        UserLoginResponse Login(string email, string password);
        User Register(User user);
        
    }
}
