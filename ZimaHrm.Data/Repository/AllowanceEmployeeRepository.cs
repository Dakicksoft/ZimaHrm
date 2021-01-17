using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class AllowanceEmployeeRepository : BaseRepository<AllowanceEmployee>, IAllowanceEmployee
    {
        public AllowanceEmployeeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
