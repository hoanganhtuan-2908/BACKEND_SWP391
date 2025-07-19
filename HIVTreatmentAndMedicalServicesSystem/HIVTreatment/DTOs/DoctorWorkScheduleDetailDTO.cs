namespace HIVTreatment.DTOs
{
    public class DoctorWorkScheduleDetailDTO
    {
        public string ScheduleID { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string SlotID { get; set; }
        public int SlotNumber { get; set; }
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; } 
        public DateTime DateWork { get; set; }
    }
}