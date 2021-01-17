using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class LeaveTypeRepository : BaseRepository<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<SelectListItem> GetAllLeaveTypeForDropDown()
        {
            return All().Select(x => new SelectListItem
            {
                Text = x.LeaveTypeName,
                Value = x.Id.ToString()
            });
        }
    }
}
