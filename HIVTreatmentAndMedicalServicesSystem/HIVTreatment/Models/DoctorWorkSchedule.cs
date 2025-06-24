using System.ComponentModel.DataAnnotations;

namespace HIVTreatment.Models
{
    public class DoctorWorkSchedule
    {
        [Key]
        public string ScheduleID { get; set; }
        public string DoctorID { get; set; }
        public string SlotID { get; set; }
        public DateTime DateWork { get; set; }
        public string Status { get; set; }
    }
}
