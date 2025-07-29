namespace HIVTreatment.Models
{
    public class Medication
    {
        public string MedicationId { get; set; }
        public string MedicationName { get; set; }
        public string DosageForm { get; set; }

        public string? Strength { get; set; }

        public string? TargetGroup { get; set; }
        public string? Advantage { get; set; }
        public string? Use { get; set; }
        public string? Note { get; set; }


    }
}
