using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class PaySlipRepository : BaseRepository<Entity.PaySlip>, IPaySlipRepository
    {
        public PaySlipRepository(AppDbContext context) : base(context)
        {
        }
    }
}
