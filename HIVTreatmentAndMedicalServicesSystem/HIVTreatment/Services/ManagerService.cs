﻿using HIVTreatment.DTOs;
using HIVTreatment.Repositories;

namespace HIVTreatment.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;
        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }
        
        public List<DoctorWorkScheduleDetailDTO> GetAllDoctorWorkSchedules()
        {
            return _managerRepository.GetAllDoctorWorkSchedules();
        }
        public DoctorWorkScheduleDetailDTO GetDoctorWorkScheduleDetail(string scheduleId)
        {
            return _managerRepository.GetDoctorWorkScheduleDetail(scheduleId);
        }

        public bool UpdateDoctorWorkSchedule(string scheduleId, EditDoctorWorkScheduleDTO dto)
            => _managerRepository.UpdateDoctorWorkSchedule(scheduleId, dto);
        public bool DeleteDoctorWorkSchedule(string scheduleId)
        => _managerRepository.DeleteDoctorWorkSchedule(scheduleId);

       

        
    }
}
