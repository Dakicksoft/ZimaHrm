using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class AllowanceTypeRepository : BaseRepository<AllowanceType>, IAllowanceTypeRepository
    {
        public AllowanceTypeRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<SelectListItem> GetAllForDropDown()
        {
            return All().Select(x => new SelectListItem
            {
                Text = x.AllowanceTypeName,
                Value = x.AllowanceTypeName.ToString()
            });
        }
    }
}
