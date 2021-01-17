using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class HolidayRepository : BaseRepository<Holiday>, IHolidayRepository
    {
        public HolidayRepository(AppDbContext context) : base(context)
        {
        }
    }
}
