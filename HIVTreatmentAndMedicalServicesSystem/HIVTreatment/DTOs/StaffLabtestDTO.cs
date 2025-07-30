namespace HIVTreatment.DTOs
{
    public class StaffLabtestDTO
    {
        // Thông tin booking
        public string BookID { get; set; }
        public DateTime BookDate { get; set; }
        public string BookingType { get; set; }
        public string Status { get; set; }

        // Thông tin bệnh nhân (Patients table)
        public string PatientID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string BloodType { get; set; }
        public string Allergy { get; set; }

        // Thông tin người dùng (Users table)
        public string Fullname { get; set; }
    }

}
