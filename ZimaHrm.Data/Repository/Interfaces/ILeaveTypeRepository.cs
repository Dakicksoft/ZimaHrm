using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface ILeaveTypeRepository : IBaseRepository<LeaveType>
    {
        IEnumerable<SelectListItem> GetAllLeaveTypeForDropDown();
    }
}
