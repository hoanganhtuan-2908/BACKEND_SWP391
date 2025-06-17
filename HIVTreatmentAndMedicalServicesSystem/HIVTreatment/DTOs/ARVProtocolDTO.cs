namespace HIVTreatment.DTOs
{
    public class ARVProtocolDTO
    {
        public string ProtocolID { get; set; }
        public string DoctorID { get; set; }
        public string MedicalRecordID { get; set; }
        public string RegimenCode { get; set; }
        public string RegimenName { get; set; }
        public string Description { get; set; }
        public int AgeRange { get; set; }
        public string ForGroup { get; set; }
    }
}
