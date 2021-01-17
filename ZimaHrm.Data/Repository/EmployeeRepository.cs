using ZimaHrm.Data.Repository.Base;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

namespace ZimaHrm.Data.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext context;

        public EmployeeRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public IEnumerable<Employee> AllByDepartmentId(Guid deptId)
        {
            if (deptId == Guid.Empty)
                return All();
            return All().Where(x => x.DepertmentId == deptId);
        }

        public IEnumerable<SelectListItem> GetAllEmployeeExceptMappingForDropDown()
        {
            var employeesInMapping = context.LeaveEmployee.ToList().Where(x=>!x.IsDelete);
            List<Employee> employees = new List<Employee>();
            foreach (var item in employeesInMapping)
            {
                var findEmployee = Find(item.EmployeeId);
                employees.Add(findEmployee);
            }
            var employeess = All().Except(employees);
            return employeess.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }

        public IEnumerable<SelectListItem> GetAllEmployeeForDropDown()
        {
            return All().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }
        public string NameById(Guid id)
        {
            var emp = Find(id);
            return emp.Name;
        }
    }
}
