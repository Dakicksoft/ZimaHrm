using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZimaHrm.Core.DataModel;
using ZimaHrm.Core.Infrastructure.Mapping;
using ZimaHrm.Core.ViewModel;
using ZimaHrm.Data.Repository.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZimaHrm.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AttendenceController : Controller
    {
        private readonly IAttendenceRepository attendenceRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IEmployeeRepository employeeRepository;

        public AttendenceController(IAttendenceRepository attendenceRepository, 
            IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
        {
            this.attendenceRepository = attendenceRepository;
            this.departmentRepository = departmentRepository;
            this.employeeRepository = employeeRepository;
        }
        // get last 7 days attendence

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Departments = departmentRepository.GetAllDepartmentForDropDown();
            //ViewBag.Months = 
            return View();
        }

        [HttpPost]
        public ActionResult Index(AttendenceReportViewModel model)
        {
            var employees = employeeRepository.AllByDepartmentId(model.DepartmentId);
           
            int days = DateTime.DaysInMonth(DateTime.Now.Year, model.Month);
            DateTime firstdayofmonth = new DateTime(DateTime.Now.Year, model.Month, 1);
            DateTime lastdayofmonth = new DateTime(DateTime.Now.Year, model.Month, 1).AddMonths(1).AddDays(-1);

            var attendences = attendenceRepository.All().Where(x => x.AttendenceDate >= firstdayofmonth && x.AttendenceDate <= lastdayofmonth).ToList();
            AttendenceReportViewModel vm = new AttendenceReportViewModel();
            vm.DepartmentId = model.DepartmentId;
            vm.Month = model.Month;

            for (int i = 1; i <= days; i++)
            {
                string currentDate = new DateTime(DateTime.Now.Year, model.Month, i).ToShortDateString();
                vm.AllCurrentMonthDate.Add(currentDate);
            }

            foreach (var item in employees)
            {
                EmployeeStatustViewModel empStatusVm = new EmployeeStatustViewModel(days, model.Month);
                empStatusVm.EmployeeId = item.Id;
                empStatusVm.EmployeeName = item.Name;
                 var attendenceWithEmployee = attendences.Where(x => x.EmployeeId ==  item.Id).OrderBy(x=>x.AttendenceDate).ToList();
                foreach (var attEmp in attendenceWithEmployee)
                {
                    string attndDate = attEmp.AttendenceDate.ToShortDateString();
                    
                    if (vm.AllCurrentMonthDate.Contains(attndDate))
                    {
                        var statusR = empStatusVm.statustViewModel.FindIndex(x=>x.Date.ToShortDateString() == attndDate);
                        empStatusVm.statustViewModel.RemoveAt(statusR);

                        var status = new StatustViewModel();
                        status.Date = attEmp.AttendenceDate;
                        if (attEmp.Status == "Present")
                        {
                            status.Status = "Present";
                        }

                        if (attEmp.Status == "Absense")
                        {
                            status.Status = "Absense";
                        }
                        empStatusVm.statustViewModel.Insert(statusR,status);
                    }
                   
                }
                vm.StatusViewModel.Add(empStatusVm);
            }
            return View(vm);
        }

        // take attendence of the
        [HttpGet]
        public ActionResult TakeAttendence()
        {
            ViewBag.Departments = departmentRepository.GetAllDepartmentForDropDown();
            return View(new AttendenceViewModel());
        }
        [HttpPost]
        public ActionResult TakeAttendence(AttendenceViewModel model)
        {
            ViewBag.Departments = departmentRepository.GetAllDepartmentForDropDown();
            var attendences = attendenceRepository.All().Any(x => x.AttendenceDate == model.AttendenceDate);
            AttendenceViewModel vm = new AttendenceViewModel();
            vm.AttendenceDate = model.AttendenceDate;
            vm.DepartmentId = model.DepartmentId;
            if (attendences)
            {
                var attendecesList = attendenceRepository.All().Where(x => x.AttendenceDate == model.AttendenceDate).ToList();
                foreach (var item in attendecesList)
                {
                    vm.AttendenceListViewModel.Add(new AttendenceListViewModel { EmployeeId = item.EmployeeId,
                        AttendenceId = item.Id, Remark = item.Reason, Status = item.Status,
                        Name = employeeRepository.NameById(item.EmployeeId)});
                }
                return View(vm);
            }
            var employees = employeeRepository.AllByDepartmentId(model.DepartmentId);
            if (employees != null)
            {
                foreach (var item in employees)
                {
                    vm.AttendenceListViewModel.Add(new AttendenceListViewModel { EmployeeId = item.Id, Name = item.Name });
                }
                return View(vm);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveAttendence(AttendenceViewModel model)
        {
            if (model.AttendenceListViewModel.Count() > 0){
                foreach (var item in model.AttendenceListViewModel)
                {
                    if (item.AttendenceId == Guid.Empty)
                    {
                        //save 
                        attendenceRepository.Insert(new AttendenceModel { AttendenceDate= model.AttendenceDate,
                            EmployeeId = item.EmployeeId, Status = item.Status, Reason = item.Remark }.Map<Data.Entity.Attendence>());
                    }
                    else
                    {
                        // update 
                        var existingAttendence = attendenceRepository.Find(item.AttendenceId);
                        if (existingAttendence != null)
                        {
                            existingAttendence.Status = item.Status;
                            existingAttendence.Reason = item.Remark;
                            existingAttendence.EmployeeId = item.EmployeeId;
                            attendenceRepository.Update(existingAttendence, existingAttendence.Id);
                        }
                    }
                }
            }
            return RedirectToAction("TakeAttendence");
        }

        #region Private Methods 
        #endregion
    }
}
