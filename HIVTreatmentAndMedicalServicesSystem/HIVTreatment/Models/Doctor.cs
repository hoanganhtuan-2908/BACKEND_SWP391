﻿using System.ComponentModel.DataAnnotations.Schema;

namespace HIVTreatment.Models
{
    public class Doctor
    {
        public string DoctorId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; } // Navigation property
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int ExperienceYears { get; set; }

    }
}
