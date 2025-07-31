using HIVTreatment.DTOs;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HIVTreatment.Repositories
{
    public interface ILabTestRepository
    {
        List<LabTest> GetAllLabTests();
        LabTest GetLabTestById(string labTestId);
        List<LabTest> GetLabTestsByPatientId(string patientId);
        void AddLabTest(LabTest labTest);
        void UpdateLabTest(LabTest labTest);
        void DeleteLabTest(string labTestId);
        BooksAppointment CreateBookingLabTest(BookingLabTestDTO dto);
        List<StaffLabtestDTO> StaffGetAllBookingsLabtest();

    }
}