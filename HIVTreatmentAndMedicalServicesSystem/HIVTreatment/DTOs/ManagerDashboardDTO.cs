public class ManagerDashboardDTO
{
    public int TotalUsers { get; set; }

    public Dictionary<string, int> UsersByRole { get; set; } = new();

    public int TotalDoctors { get; set; }

    public int TotalPatients { get; set; }

    public int TotalLabTests { get; set; }

    public int TotalTreatmentPlans { get; set; }

    public int TotalPrescriptions { get; set; }
}
