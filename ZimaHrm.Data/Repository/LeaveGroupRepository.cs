using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class LeaveGroupRepository : BaseRepository<LeaveGroup>, ILeaveGroupRepository
    {
        public LeaveGroupRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<SelectListItem> GetAllLeaveGroupForDropDown()
        {
            return All().Select(x => new SelectListItem
            {
                Text = x.LeaveGroupName,
                Value = x.Id.ToString()
            });
        }
    }
}
