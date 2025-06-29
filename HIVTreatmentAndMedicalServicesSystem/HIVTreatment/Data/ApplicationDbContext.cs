using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;

namespace HIVTreatment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<BooksAppointment> BooksAppointments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlan { get; set; }


        public DbSet<ARVProtocol> ARVProtocol { get; set; }
        public DbSet<Slot> Slot { get; set; }
        public DbSet<DoctorWorkSchedule> DoctorWorkSchedules { get; set; }
        public DbSet<Prescription> Prescription { get; set; }
        

    }
}
