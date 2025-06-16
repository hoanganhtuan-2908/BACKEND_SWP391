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
    }
}
