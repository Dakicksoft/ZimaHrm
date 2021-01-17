using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;

namespace ZimaHrm.Data.Repository.Interfaces
{
    public interface IAllowanceTypeRepository : IBaseRepository<AllowanceType>
    {
        IEnumerable<SelectListItem> GetAllForDropDown();
    }
}
