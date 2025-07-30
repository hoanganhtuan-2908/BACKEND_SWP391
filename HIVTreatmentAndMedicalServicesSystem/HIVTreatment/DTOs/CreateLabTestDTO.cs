public class CreateLabTestDTO
{
    public string RequestID { get; set; }
    //public string TreatmentPlantID { get; set; }
    public string TestName { get; set; }
    public string TestType { get; set; }
    public string ResultValue { get; set; }
    public int? CD4Initial { get; set; }
    public int? ViralLoadInitial { get; set; }
    //public string Status { get; set; } để mặc định khi tạo là: Đang xử lý
    public string Description { get; set; }
}