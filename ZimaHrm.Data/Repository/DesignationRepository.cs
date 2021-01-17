using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository.Base
{
    public class DesignationRepository : BaseRepository<Designation>, IDesignationRepository
    {
        public DesignationRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Designation> AllDesignationByDepartmentId(Guid dptId)
        {
            return All().Where(x => x.DepartmentId == dptId).ToList();
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

        public IEnumerable<SelectListItem> GetAllDesignationForDropDown()
        {
            return All().ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }

       
    }
}
