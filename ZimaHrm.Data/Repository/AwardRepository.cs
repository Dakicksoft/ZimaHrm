using System;
using System.Collections.Generic;
using System.Linq;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class AwardRepository : BaseRepository<Award>, IAwardRepository
    {
        public AwardRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Award> AwardListByEmployeeId(Guid empId)
        {
            return base.All().Where(s => s.EmployeeId == empId).AsEnumerable();
        }
    }
}
