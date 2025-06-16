using HIVTreatment.DTOs;
using HIVTreatment.Models;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository iUserRepository;
        private readonly IPatientRepository iPatientRepository;

        public ProfileService(IUserRepository userRepository, IPatientRepository patientRepository)
        {
            iUserRepository = userRepository;
            iPatientRepository = patientRepository;
        }

        public bool UpdateProfile(EditProfileUserDTO editProfileUserDTO)
        {
            var user = iUserRepository.GetByUserId(editProfileUserDTO.UserId);
            if (user == null)
            {
                return false; // User not found
            }

            user.Fullname = editProfileUserDTO.Fullname;
            iUserRepository.Update(user);

            var patient = iPatientRepository.GetByPatientId(editProfileUserDTO.UserId);
            if (patient == null)
            {
                var lastPatient = iPatientRepository.GetLastPatientId();
                int nextId = 1;
                if (lastPatient != null && lastPatient.PatientID?.Length >= 8)
                {
                    string numberPart = lastPatient.PatientID.Substring(2);
                    if (int.TryParse(numberPart, out int parsed))
                    {
                        nextId = parsed + 1;
                    }
                }
                string newPatientID = "PT" + nextId.ToString("D6");
                patient = new Patient
                {
                    PatientID = newPatientID,
                    UserID = editProfileUserDTO.UserId,
                    DateOfBirth = editProfileUserDTO.DateOfBirth,
                    Gender = editProfileUserDTO.Gender,
                    Phone = editProfileUserDTO.Phone,
                    BloodType = editProfileUserDTO.BloodType,
                    Allergy = editProfileUserDTO.Allergy,
                };
                // Use Add instead of Update for new entities
                iPatientRepository.Add(patient);
            }
            else
            {
                // Update existing patient data
                patient.DateOfBirth = editProfileUserDTO.DateOfBirth;
                patient.Gender = editProfileUserDTO.Gender;
                patient.Phone = editProfileUserDTO.Phone;
                patient.BloodType = editProfileUserDTO.BloodType;
                patient.Allergy = editProfileUserDTO.Allergy;
                iPatientRepository.Update(patient);
            }
            return true;
        }

    }
}