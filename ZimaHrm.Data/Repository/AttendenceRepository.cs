using System;
using System.Collections.Generic;
using System.Linq;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class AttendenceRepository : BaseRepository<Attendence>, IAttendenceRepository
    {
        public AttendenceRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Attendence> AttendenceListByEmployee(Guid empId)
        {
            return base.All().Where(s => s.EmployeeId == empId).AsEnumerable();
        }
    }
}
