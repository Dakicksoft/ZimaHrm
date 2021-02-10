using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ZimaHrm.Core.DataModel;
using ZimaHrm.Core.Infrastructure.Mapping;
using ZimaHrm.Core.ViewModel;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;
using ZimaHrm.Data.Repository.PaySlip;
using ZimaHrm.Web.Services;

namespace ZimaHrm.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeDashboard dashboardRepository;
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly IAwardRepository awardRepository;
        private readonly ILeaveApplicationRepository leaveApplicationRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IEmployeePaySlipRepository employeePaySlipRepository;
        private readonly IPaySlipAllowanceRepository paySlipAllowanceRepository;
        private readonly IAttendenceRepository attendenceRepository;
        private readonly IAuthenticatedUserService _currentUser;
        private readonly UserManager<User> _userManager;
        public EmployeeController(IEmployeeDashboard dashboardRepository,
                                  ILeaveTypeRepository leaveTypeRepository,
                                  IAwardRepository awardRepository,
                                  ILeaveApplicationRepository leaveApplicationRepository,
                                  IEmployeeRepository employeeRepository,
                                  IEmployeePaySlipRepository employeePaySlipRepository,
                                  IPaySlipAllowanceRepository paySlipAllowanceRepository,
                                  IAttendenceRepository attendenceRepository,
                                  IAuthenticatedUserService currentUser, 
                                  UserManager<User> userManager)

        {
            this.dashboardRepository = dashboardRepository;
            this.leaveTypeRepository = leaveTypeRepository;
            this.awardRepository = awardRepository;
            this.leaveApplicationRepository = leaveApplicationRepository;
            this.employeeRepository = employeeRepository;
            this.employeePaySlipRepository = employeePaySlipRepository;
            this.paySlipAllowanceRepository = paySlipAllowanceRepository;
            this.attendenceRepository = attendenceRepository;
            _currentUser = currentUser;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            EmployeeViewModel empViewModel = new EmployeeViewModel
            {
                Notices = dashboardRepository.Notices().ToList().Map<ICollection<NoticeModel>>(),
                Holidays = dashboardRepository.Holidays().ToList().Map<ICollection<HolidayModel>>()
            };
            ;
            empViewModel.Employee = employeeRepository.All().FirstOrDefault(x => x.Id ==Guid.Parse(_currentUser.UserId)).Map<EmployeeModel>();
            empViewModel.RecentAwardList = dashboardRepository.RecentAwardList(Guid.Parse(_currentUser.UserId)).ToList().Map<List<AwardModel>>();
            empViewModel.RecentPaySlipList = dashboardRepository.RecentPaySlipList(Guid.Parse(_currentUser.UserId)).ToList().Map<List<EmployeePaySlipModel>>();
            empViewModel.TotalAbsenceInMonth = dashboardRepository.TotalAbsenceInMonth(Guid.Parse(_currentUser.UserId));
            empViewModel.TotalAttendencInMonth = dashboardRepository.TotalAttendencInMonth(Guid.Parse(_currentUser.UserId));
            empViewModel.TotalLeaveInMonth = dashboardRepository.TotalLeaveInMonth(Guid.Parse(_currentUser.UserId));
            return View(empViewModel);
        }
        [HttpGet]
        public async Task<ActionResult> LeaveApplicationList()
        {
            var leaveApplications = await leaveApplicationRepository.All()
                                                                  .AsSplitQuery()
                                                                  .Include(c => c.LeaveType)
                                                                  .Where(x => x.EmployeeId == Guid.Parse(_currentUser.UserId))
                                                                  .ToListAsync()
                                                                  .ConfigureAwait(false);

            return View(leaveApplications.Map<List<LeaveApplicationModel>>());
        }
        [HttpGet]
        public ActionResult LeaveApplication()
        {

            ViewBag.LeaveTypes = leaveTypeRepository.GetAllLeaveTypeForDropDown();
            return View();
        }
        [HttpPost]
        public ActionResult LeaveApplication(LeaveApplicationModel model)
        {
            model.EmployeeId = Guid.Parse(_currentUser.UserId);
            if (!ModelState.IsValid)
            {
                ViewBag.LeaveTypes = leaveTypeRepository.GetAllLeaveTypeForDropDown();
                return View(model);
            }
            leaveApplicationRepository.Insert(model.Map<LeaveApplication>());
            return RedirectToAction(nameof(LeaveApplicationList));
        }

        [HttpGet]
        public IActionResult EditLeaveApplication(Guid id)
        {
            var leaveApp = leaveApplicationRepository.Find(id);
            if (leaveApp != null && 
                (leaveApp.Status == "Pending" || leaveApp.Status == "Reject") && 
                leaveApp.EmployeeId == Guid.Parse(_currentUser.UserId))
            {
                ViewBag.LeaveTypes = leaveTypeRepository.GetAllLeaveTypeForDropDown();
                return View(leaveApp.Map<LeaveApplicationModel>());
            }
            return RedirectToAction(nameof(LeaveApplicationList));
        }
        [HttpPost]
        public IActionResult EditLeaveApplication(LeaveApplicationModel model)
        {
            var leaveApp = leaveApplicationRepository.Find(model.Id);
            if (ModelState.IsValid && leaveApp.EmployeeId == Guid.Parse(_currentUser.UserId))
            {
                leaveApp.LeaveDate = model.LeaveDate;
                leaveApp.LeaveTypeId = model.LeaveTypeId;
                leaveApp.Reason = model.Reason;
                leaveApp.Status = model.Status;
                leaveApplicationRepository.Update(leaveApp, model.Id);
                return RedirectToAction(nameof(LeaveApplicationList));
            }
            ViewBag.LeaveTypes = leaveTypeRepository.GetAllLeaveTypeForDropDown();
            return View(model);
        }
        [HttpGet]
        public IActionResult DeleteLeaveApplication(Guid id)
        {
            var leaveApp = leaveApplicationRepository.Find(id);
            if (leaveApp != null && 
                (leaveApp.Status == "Pending" || leaveApp.Status == "Reject") && 
                leaveApp.EmployeeId == Guid.Parse(_currentUser.UserId))
            {
                leaveApplicationRepository.Delete(leaveApp);
                return RedirectToAction(nameof(LeaveApplicationList));
            }
            return RedirectToAction(nameof(LeaveApplicationList));
        }

        public IActionResult Profile()
        {
            var employee = employeeRepository.All()
                                             .AsSplitQuery()
                                             .Include(x => x.Designation)
                                             .FirstOrDefault(x => x.Id ==Guid.Parse(_currentUser.UserId));
            return View(employee.Map<EmployeeModel>());
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {

            var user = await _userManager.FindByIdAsync(_currentUser.UserId);
            if (user != null)
            {
                var passwordValidator = new PasswordValidator<User>();
                var result = await passwordValidator.ValidateAsync(_userManager, user, model.CurrentPass);

                if (result.Succeeded)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPass);
                    await _userManager.UpdateAsync(user);
                    TempData["Msg"] = "Password changed successfully!";
                    return View();
                }
            }
            TempData["FMsg"] = "Current password doesn't match, Failed to changed password! ";
            return View(model);
        }

        public async Task<IActionResult> AwardList()
        {
            var awards = await awardRepository.All()
                                            .Where(x => x.EmployeeId == Guid.Parse(_currentUser.UserId))
                                            .ToListAsync()
                                            .ConfigureAwait(false);
            return View(awards.Map<List<AwardModel>>());
        }
        [HttpGet]
        public IActionResult Attendence()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Attendence(EmployeeAttandenceViewModel model)
        {
            int days = DateTime.DaysInMonth(DateTime.Now.Year, model.Month);
            DateTime firstdayofmonth = new DateTime(DateTime.Now.Year, model.Month, 1);
            DateTime lastdayofmonth = new DateTime(DateTime.Now.Year, model.Month, 1).AddMonths(1).AddDays(-1);

            var attendences = attendenceRepository.All()
                                                  .Where(x => x.AttendenceDate >= firstdayofmonth &&
                                                              x.AttendenceDate <= lastdayofmonth &&
                                                              x.EmployeeId == Guid.Parse(_currentUser.UserId))
                                                  .OrderBy(x => x.AttendenceDate)
                                                  .ToList();


            AttendenceReportViewModel vm = new AttendenceReportViewModel();
            vm.Month = model.Month;

            for (int i = 1; i <= days; i++)
            {
                string currentDate = new DateTime(DateTime.Now.Year, model.Month, i).ToShortDateString();
                vm.AllCurrentMonthDate.Add(currentDate);
            }

            EmployeeStatustViewModel empStatusVm = new EmployeeStatustViewModel(days, model.Month);
            empStatusVm.EmployeeId = Guid.Parse(_currentUser.UserId);
            empStatusVm.EmployeeName = _currentUser.Name;
            foreach (var attEmp in attendences)
            {
                string attndDate = attEmp.AttendenceDate.ToShortDateString();

                if (vm.AllCurrentMonthDate.Contains(attndDate))
                {
                    var statusR = empStatusVm.statustViewModel.FindIndex(x => x.Date.ToShortDateString() == attndDate);
                    empStatusVm.statustViewModel.RemoveAt(statusR);

                    var status = new StatustViewModel {Date = attEmp.AttendenceDate};
                    if (attEmp.Status == "Present")
                    {
                        status.Status = "Present";
                    }

                    if (attEmp.Status == "Absense")
                    {
                        status.Status = "Absense";
                    }
                    empStatusVm.statustViewModel.Insert(statusR, status);
                }

            }
            vm.StatusViewModel.Add(empStatusVm);
            return View(vm);
        }

        public async Task<IActionResult> PaySlipList()
        {
            var employeePaySlips = await employeePaySlipRepository.All()
                                                                  .Where(x => x.EmployeeId == Guid.Parse(_currentUser.UserId))
                                                                  .ToListAsync()
                                                                  .ConfigureAwait(false);

            return View(employeePaySlips.Map<List<EmployeePaySlipModel>>());
        }

        public ActionResult EmpPaySlipDetails(Guid id)
        {
            var employeePaySlip = employeePaySlipRepository.All()
                                                           .AsSplitQuery()
                                                           .Include(x => x.Employee)
                                                           .Include(x => x.PaySlip)
                                                           .FirstOrDefault(x => x.Id == id && x.EmployeeId == Guid.Parse(_currentUser.UserId));
            
            employeePaySlip.PaySlipAllowances = paySlipAllowanceRepository.All()
                                                                          .Where(x => x.EmployeePaySlipId == employeePaySlip.Id)
                                                                          .ToList();
            return View(employeePaySlip.Map<EmployeePaySlipModel>());
        }
        public ActionResult PrintPaySlip(Guid id)
        {
            var employeePaySlip = employeePaySlipRepository.All()
                                                           .AsSplitQuery()
                                                           .Include(x => x.Employee)
                                                           .Include(x => x.PaySlip)
                                                           .FirstOrDefault(x => x.Id == id);
            employeePaySlip.PaySlipAllowances = paySlipAllowanceRepository.All()
                                                                          .Where(x => x.EmployeePaySlipId == employeePaySlip.Id)
                                                                          .ToList();
            //return View(employeePaySlip);
            return new ViewAsPdf("PrintPaySlip", employeePaySlip)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
 

    }
}
