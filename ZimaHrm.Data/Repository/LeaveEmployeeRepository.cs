using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class LeaveEmployeeRepository : BaseRepository<LeaveEmployee>, ILeaveEmployeeRepository
    {
        public LeaveEmployeeRepository(AppDbContext context) : base(context)
        {
        }

        
    }
}
