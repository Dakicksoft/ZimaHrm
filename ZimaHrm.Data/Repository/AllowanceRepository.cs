using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class AllowanceRepository : BaseRepository<Allowance>, IAllowanceRepository
    {
        public AllowanceRepository(AppDbContext context) : base(context)
        {
        }
    }
}
