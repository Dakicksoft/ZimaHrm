using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Base;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(AppDbContext context) : base(context)
        {
        }

        

        public bool AlreadyExist(string deptName, Guid id)
        {
            if (id != Guid.Empty)
            {
                var brand = All().FirstOrDefault(x => x.Name == deptName && id != x.Id);
                return brand != null;
            }
            else
            {
                var brand = All().FirstOrDefault(x => x.Name == deptName);
                return brand != null;
            }
        }

        public IEnumerable<SelectListItem> GetAllDepartmentForDropDown()
        {
            return All().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }

       
    }
}
