using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Books")]
public class BooksAppointment
{
    [Key]
    [StringLength(10)]
    public string BookID { get; set; }
    public string PatientID { get; set; }
    public string DoctorID { get; set; }
    public string ServiceID { get; set; }
    public DateTime BookDate { get; set; }
    public string Status { get; set; }
    public string Note { get; set; }
}