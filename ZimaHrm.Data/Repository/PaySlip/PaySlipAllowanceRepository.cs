using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.PaySlip
{
    public class PaySlipAllowanceRepository : BaseRepository<PaySlipAllowance>, IPaySlipAllowanceRepository
    {
        public PaySlipAllowanceRepository(AppDbContext context) : base(context)
        {
        }
    }
}
