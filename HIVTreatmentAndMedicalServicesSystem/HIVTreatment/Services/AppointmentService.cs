//using HIVTreatment.Data;
//using HIVTreatment.Models;
//using HIVTreatment.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace HIVTreatment.Services
//{
//    public class AppointmentService : IAppointmentService
//    {
//        private readonly ApplicationDbContext _context;

//        public AppointmentService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // Pseudocode plan:
//        // 1. Check if dto.AppointmentDate is a valid DateTime and not default(DateTime).
//        // 2. Ensure dto.AppointmentDate is not in the past (optional, but common for appointments).
//        // 3. If validation fails, throw an exception or return a result indicating the error.
//        // 4. Otherwise, proceed to add the appointment as before.

//        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
//        {
//            // Validate AppointmentDate
//            if (dto.AppointmentDate == default)
//                throw new ArgumentException("AppointmentDate is required.");

//            if (dto.AppointmentDate < DateTime.UtcNow)
//                throw new ArgumentException("AppointmentDate cannot be in the past.");

//            var appointment = new Appointment
//            {
//                AppointmentId = Guid.NewGuid().ToString(),
//                PatientId = dto.PatientId,
//                DoctorId = dto.DoctorId,
//                AppointmentDate = dto.AppointmentDate,
//                Status = "Pending",
//                Notes = dto.Notes,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _context.Appointments.AddAsync(appointment);
//            await _context.SaveChangesAsync();
//            return appointment;
//        }

//        public async Task<Appointment> UpdateAppointmentAsync(string appointmentId, UpdateAppointmentDto dto)
//        {
//            var appointment = await _context.Appointments.FindAsync(appointmentId);
//            if (appointment == null)
//                return null;

//            appointment.Status = dto.Status;
//            appointment.Notes = dto.Notes;
//            appointment.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return appointment;
//        }

//        public async Task<Appointment> GetAppointmentByIdAsync(string appointmentId)
//        {
//            return await _context.Appointments
//                .Include(a => a.Patient)
//                .Include(a => a.Doctor)
//                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(string patientId)
//        {
//            return await _context.Appointments
//                .Include(a => a.Doctor)
//                .Where(a => a.PatientId == patientId)
//                .OrderByDescending(a => a.AppointmentDate)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(string doctorId)
//        {
//            return await _context.Appointments
//                .Include(a => a.Patient)
//                .Where(a => a.DoctorId == doctorId)
//                .OrderByDescending(a => a.AppointmentDate)
//                .ToListAsync();
//        }

//        public async Task<bool> DeleteAppointmentAsync(string appointmentId)
//        {
//            var appointment = await _context.Appointments.FindAsync(appointmentId);
//            if (appointment == null)
//                return false;

//            _context.Appointments.Remove(appointment);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//} 