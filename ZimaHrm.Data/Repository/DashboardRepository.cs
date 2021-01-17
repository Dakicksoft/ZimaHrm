using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;
        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Notice>> LastFiveNotificationsAsync()
        {
            return await Task.FromResult(_context.Notices
                           .AsNoTracking()
                           .OrderByDescending(x => x.CreatedUtc)
                           .Take(5));
        }
        public async Task<IEnumerable<Holiday>> LastFiveHolidaysAsync()
        {
            return await Task.FromResult(_context.Holidays
                                                 .AsNoTracking()
                                                 .OrderByDescending(x => x.CreatedUtc)
                                                 .Take(5));
        }

        public async Task<int> TotalAbsentAsync()
        {
            return await _context.Attendences
                           .AsNoTracking()
                           .Where(x => x.Status == "Absense" &&
                                       x.AttendenceDate.Date == DateTime.Now.Date)
                           .CountAsync()
                           .ConfigureAwait(false);
        }

        public async Task<int> TotalDepartmentAsync()
        {
            return await _context.Depertments
                                 .AsNoTracking()
                                 .CountAsync()
                                 .ConfigureAwait(false);
        }

        public async Task<int> TotalEmplooyeeAsync()
        {
            return await _context.Employees
                                 .AsNoTracking()
                                 .CountAsync()
                                 .ConfigureAwait(false);
        }

        public async Task<int> TotalPresentAsync()
        {
            return await _context.Attendences
                                 .AsNoTracking()
                                 .CountAsync(x => x.Status == "Present" &&
                                             x.AttendenceDate.Date == DateTime.Now.Date)
                                 .ConfigureAwait(false);
        }
    }
}
