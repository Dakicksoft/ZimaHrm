using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class EmployeeDashboard : IEmployeeDashboard
    {
        private DateTime startDate;
        private DateTime endDate;
        private readonly AppDbContext context;

        public EmployeeDashboard(AppDbContext context)
        {
            this.context = context;
          
            // current month days
            //day = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

        }
        public IEnumerable<Holiday> Holidays()
        {
            return context.Holidays.AsNoTracking().Take(5).OrderByDescending(x => x.CreatedUtc);
        }

        public IEnumerable<Notice> Notices()
        {
            return context.Notices.AsNoTracking().Take(5).OrderByDescending(x => x.CreatedUtc);
        }
       
        public IEnumerable<Award> RecentAwardList(Guid id)
        {
            return context.Awards.AsNoTracking().Take(5).
                OrderByDescending(x => x.CreatedUtc).Where(x =>x.EmployeeId == id).ToList();
        }

        public IEnumerable<EmployeePaySlip> RecentPaySlipList(Guid id)
        {
            return context.EmployeePaySlip.AsNoTracking().Take(5).
               OrderByDescending(x => x.CreatedUtc).Where(x => x.EmployeeId == id).ToList();
        }

      

        // need to modify
        public int TotalAbsenceInMonth(Guid id)
        {
            return context.Attendences.AsNoTracking().Where(x => x.Status == "Absense" && x.EmployeeId == id &&
            x.AttendenceDate >= startDate && x.AttendenceDate <= endDate).Count();
           
        }

       
        public int TotalAttendencInMonth(Guid id)
        {
            return context.Attendences.AsNoTracking().Where(x => x.Status == "Present" && x.EmployeeId == id &&
           x.AttendenceDate >= startDate && x.AttendenceDate <= endDate).Count();
        }

      
        public int TotalLeaveInMonth(Guid id)
        {
            return context.LeaveApplication.AsNoTracking().Where(x => x.Status == "Approve"
                                                    && x.EmployeeId == id && 
                                                    x.LeaveDate >= startDate && x.LeaveDate <= endDate).Count();
        }
    }
}
