namespace HIVTreatment.DTOs
{
    public class EditProfileUserDTO
    {
        public string UserId { get; set; }
        public string Fullname { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string BloodType { get; set; }
        public string Allergy { get; set; }
    }
}
