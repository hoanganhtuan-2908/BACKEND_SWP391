using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HIVTreatment.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public User GetByUserId(string userId)
        {
            return _userRepository.GetByUserId(userId);
        }

        //login jwt token
        public UserLoginResponse Login(string email, string password)
        {
            try
            {
                var user = _userRepository.GetByEmail(email);
                if (user == null || user.Password != password)
                    return null;
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                
                if (key.Length < 16)
                {
                    throw new Exception("JWT Key must be at least 16 characters long");
                }
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId)
                };

                if (!double.TryParse(_configuration["Jwt:ExpiryInHours"], out double expiryHours))
                {
                    expiryHours = 3; //timeout
                }
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(expiryHours),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return new UserLoginResponse
                {
                    UserId =  user.UserId, 
                    RoleId  = user.RoleId, 
                    Fullname =  user.Fullname,
                    Email =  user.Email,
                    Token = tokenHandler.WriteToken(token)
                };


            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Error creating token: {ex.Message}");
                throw;
            }
        }

        public User Register(User user)
        {
            if (_userRepository.EmailExists(user.Email)) return null;

            var lastUser = _userRepository.GetLastUser();
            int nextId = 1;
            if (lastUser != null)
            {
                string numberPart = lastUser.UserId.Substring(3);
                if (int.TryParse(numberPart, out int parsed))
                    nextId = parsed + 1;
            }

            user.UserId = "UID" + nextId.ToString("D6");
            _userRepository.Add(user);
            return user;
        }

        public bool ResetPassword(string email, string newPassword)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null) return false;

            _userRepository.UpdatePassword(email, newPassword);
            return true;
        }
    }
}