using System.Collections.Generic;
using System;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IEmployeeDashboard
    {
        int TotalAttendencInMonth(Guid id);
        int TotalAbsenceInMonth(Guid id);
        int TotalLeaveInMonth(Guid id);

        IEnumerable<Notice> Notices();
        IEnumerable<Holiday> Holidays();
        IEnumerable<EmployeePaySlip> RecentPaySlipList(Guid id);
        IEnumerable<Award> RecentAwardList(Guid id);
    }
}
