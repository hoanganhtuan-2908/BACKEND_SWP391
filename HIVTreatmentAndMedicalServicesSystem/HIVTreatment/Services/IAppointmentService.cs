//using HIVTreatment.Models;
//using HIVTreatment.Models.Entities;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace HIVTreatment.Services
//{
//    public interface IAppointmentService
//    {
//        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto);
//        Task<Appointment> UpdateAppointmentAsync(string appointmentId, UpdateAppointmentDto dto);
//        Task<Appointment> GetAppointmentByIdAsync(string appointmentId);
//        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(string patientId);
//        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(string doctorId);
//        Task<bool> DeleteAppointmentAsync(string appointmentId);
//    }
//} 