using HIVTreatment.Data;
using HIVTreatment.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HIVTreatment.Services
{
    public class StaffAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public StaffAppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetCurrentUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public bool IsStaff(User user) => user.RoleId == "R004";

        public List<BooksAppointment> GetCompletedAppointments()
        {
            return _context.BooksAppointments
                .Where(a => a.Status == "Đã khám")
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .ToList();
        }

        public List<BooksAppointment> GetSuccessfulAppointments()
        {
            return _context.BooksAppointments
                .Where(a => a.Status == "Thành công")
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .ToList();
        }

        public List<BooksAppointment> GetCancelledAppointments()
        {
            return _context.BooksAppointments
                .Where(a => a.Status == "Đã hủy")
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .ToList();
        }

        public List<BooksAppointment> GetAllRelevantAppointments()
        {
            return _context.BooksAppointments
                .Where(a => a.Status == "Thành công" || a.Status == "Đã hủy" || a.Status == "Đã khám")
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .ToList();
        }
    }
}
