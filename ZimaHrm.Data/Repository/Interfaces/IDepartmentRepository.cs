using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Entity;
using System;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        IEnumerable<SelectListItem> GetAllDepartmentForDropDown();

        bool AlreadyExist(string deptName, Guid id);
    }
}
