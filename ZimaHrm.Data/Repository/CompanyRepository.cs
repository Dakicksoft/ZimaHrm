using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository.Base
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext context) : base(context)
        {

        }
    }
}
