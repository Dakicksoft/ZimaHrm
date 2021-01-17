using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ZimaHrm.Data.Repository.Base;
using System;
using ZimaHrm.Data.Entity;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        IEnumerable<Employee> AllByDepartmentId(Guid deptId);
        IEnumerable<SelectListItem> GetAllEmployeeForDropDown();
        IEnumerable<SelectListItem> GetAllEmployeeExceptMappingForDropDown();
        string NameById(Guid id);
    }
}
