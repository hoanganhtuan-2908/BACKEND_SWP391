namespace HIVTreatment.DTOs
{
    public class ReExaminationAppointment
    {
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public string BookingType { get; set; }
        public DateTime BookDate { get; set; }
        public string Note { get; set; }
    }
}
