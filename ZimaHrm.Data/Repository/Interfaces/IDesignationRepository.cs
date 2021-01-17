using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IDesignationRepository : IBaseRepository<Designation>
    {
        IEnumerable<SelectListItem> GetAllDesignationForDropDown();
        IEnumerable<Designation> AllDesignationByDepartmentId(Guid dptId);
        bool AlreadyExist(string deptName, Guid id);
    }
}
