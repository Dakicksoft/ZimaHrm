using System.Collections.Generic;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Entity;
using System;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IAwardRepository : IBaseRepository<Award>
    {
        IEnumerable<Award> AwardListByEmployeeId(Guid empId);
    }
}
