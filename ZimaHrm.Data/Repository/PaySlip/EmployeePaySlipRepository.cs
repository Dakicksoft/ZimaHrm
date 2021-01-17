using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;

namespace ZimaHrm.Data.Repository.PaySlip
{
    public class EmployeePaySlipRepository : BaseRepository<EmployeePaySlip>, IEmployeePaySlipRepository
    {
        public EmployeePaySlipRepository(AppDbContext context) : base(context)
        {
        }
    }
}
