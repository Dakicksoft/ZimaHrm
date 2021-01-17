using System.Collections.Generic;
using System;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IAttendenceRepository : IBaseRepository<Attendence>
    {
        IEnumerable<Attendence> AttendenceListByEmployee(Guid empId);
    }
}
