using System.ComponentModel.DataAnnotations.Schema;

namespace HIVTreatment.Models
{
    [Table("TreatmentPlan")] // <- Bắt buộc để EF nhận đúng tên bảng
    public class TreatmentPlan
    {
        public string TreatmentPlanID { get; set; }
        public string PatientID { get; set; }
        public string DoctorID { get; set; }
        public string ARVProtocol { get; set; }
        public int TreatmentLine { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentResult { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}
