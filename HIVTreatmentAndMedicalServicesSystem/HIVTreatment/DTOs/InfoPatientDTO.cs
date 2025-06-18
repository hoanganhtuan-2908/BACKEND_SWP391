namespace HIVTreatment.DTOs
{
    public class InfoPatientDTO
    {
        public string UserID { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string BloodType { get; set; }
        public string Allergy { get; set; }
    }
}
