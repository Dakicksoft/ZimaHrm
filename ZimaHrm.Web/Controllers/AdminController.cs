using System;
using System.Linq;
using System.Threading.Tasks;
using ZimaHrm.Core.DataModel;
using ZimaHrm.Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZimaHrm.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using ZimaHrm.Data.Entity;
using ZimaHrm.Core.Infrastructure.Mapping;
using System.Collections.Generic;
using ZimaHrm.Web.Services;

namespace ZimaHrm.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHolidayRepository _holidayRepository;
        private readonly IAwardRepository _awardRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly INoticeRepository _noticeRepository;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ILeaveGroupRepository _leaveGroupRepository;
        private readonly ILeaveEmployeeRepository _leaveEmployeeRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ILeaveApplicationRepository _leaveApplicationRepository;
        private readonly IAuthenticatedUserService _currentUser;
        private readonly UserManager<User> _userManager;

        public AdminController(IHolidayRepository holidayRepository, IAwardRepository awardRepository,
            IEmployeeRepository employeeRepository, INoticeRepository noticeRepository,
            IDashboardRepository dashboardRepository, ILeaveGroupRepository leaveGroupRepository,
            ILeaveEmployeeRepository leaveEmployeeRepository, ILeaveTypeRepository leaveTypeRepository,
            ILeaveApplicationRepository leaveApplicationRepository, UserManager<User> userManager, IAuthenticatedUserService authenticatedUser)
        {
            this._holidayRepository = holidayRepository;
            this._awardRepository = awardRepository;
            this._employeeRepository = employeeRepository;
            this._noticeRepository = noticeRepository;
            this._dashboardRepository = dashboardRepository;
            this._leaveGroupRepository = leaveGroupRepository;
            this._leaveEmployeeRepository = leaveEmployeeRepository;
            this._leaveTypeRepository = leaveTypeRepository;
            this._leaveApplicationRepository = leaveApplicationRepository;
            _userManager = userManager;
            _currentUser = authenticatedUser;
        }

        // admin dashboard
        public async Task<IActionResult> Index()
        {
            DashboardViewModel vm = new DashboardViewModel
            {
                TotalEmployee = await _dashboardRepository.TotalEmplooyeeAsync(),
                TotalDept = await _dashboardRepository.TotalDepartmentAsync(),
                PresentCountToday = await _dashboardRepository.TotalPresentAsync(),
                AbsenceCountToday = await _dashboardRepository.TotalAbsentAsync(),
                Notices = (await _dashboardRepository.LastFiveNotificationsAsync()).Map<IList<NoticeModel>>(),
                Holidays = (await _dashboardRepository.LastFiveHolidaysAsync()).Map<IList<HolidayModel>>()
            };
            return View(vm);
        }

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


        #region Holiday
        [HttpGet]
        public async Task<IActionResult> Holidays()
        {
            DateTime dt = DateTime.Now;
            string currentMonth = dt.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture);

            var holidays =await _holidayRepository.All()
                                            .Where(x => x.Month == currentMonth)
                                            .ToListAsync()
                                            .ConfigureAwait(false);

            return View(holidays.Map<List<HolidayModel>>());
        }
        [HttpPost]
        public async Task<JsonResult> Holidays(string month)
        {
            var holidays = await _holidayRepository.All()
                                            .Where(x => x.Month == month)
                                            .ToListAsync()
                                            .ConfigureAwait(false);

            return Json(holidays.Map<List<HolidayModel>>());
        }
        [HttpGet]
        public ActionResult AddHoliday()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHoliday(HolidayModel model)
        {
            if (ModelState.IsValid)
            {
                model.Month = model.Date.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture);
                model.Day = model.Date.DayOfWeek.ToString();
                _holidayRepository.Insert(model.Map<Holiday>());
                return RedirectToAction("holidays");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult EditHoliday(Guid holidayId)
        {
            var holiday = _holidayRepository.Find(holidayId);
            if (holiday != null)
            {
                return View(holiday.Map<HolidayModel>());
            }
            return RedirectToAction(nameof(Holidays));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHoliday(HolidayModel model)
        {
            model.Month = model.Date.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture);
            model.Day = model.Date.DayOfWeek.ToString();
            var holiday = _holidayRepository.Find(model.Id);
            if (ModelState.IsValid && holiday != null)
            {
                holiday.Date = model.Date;
                holiday.Day = model.Day;
                holiday.Month = model.Month;
                _holidayRepository.Update(holiday, model.Id);
                return RedirectToAction(nameof(Holidays));
            }
            return RedirectToAction(nameof(Holidays));
        }
        public IActionResult DeleteHoliday(Guid id)
        {
            var holiday = _holidayRepository.Find(id);
            if (holiday != null)
            {
                _holidayRepository.Delete(holiday);
                return RedirectToAction(nameof(Holidays));
            }
            return RedirectToAction(nameof(Holidays));
        }

        #endregion

        #region Award
        public async Task<ActionResult> AwardList()
        {
            var awards = await _awardRepository.All()
                                       .AsSplitQuery()
                                       .Include(x => x.Employee)
                                       .Select(s=>new AwardModel
                                       { 
                                        AwardTitle = s.AwardTitle,
                                        CreatedBy=s.CreatedBy,
                                        CreatedUtc=s.CreatedUtc,
                                        Date=s.Date,
                                        EmployeeId=s.EmployeeId,
                                        EmployeeModel=new EmployeeModel
                                        { 
                                         Name=s.Employee.Name,
                                         Id=s.Employee.Id
                                        },
                                        Gift=s.Gift,
                                        Id=s.Id,
                                        IsDelete=s.IsDelete,
                                        LastModifiedBy=s.LastModifiedBy,
                                        LastModifiedUtc=s.LastModifiedUtc,
                                        Month=s.Month,
                                        Price=s.Price
                                       })
                                       .ToListAsync()
                                       .ConfigureAwait(false);
            return View(awards);
        }
        [HttpGet]
        public ActionResult AddAward()
        {
            ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAward(AwardModel model)
        {
            model.Month = model.Date.ToString("MMMM");
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
                return View(model);
            }
            _awardRepository.Insert(new Award
            { 
             AwardTitle=model.AwardTitle,
             CreatedBy = _currentUser.UserId,
             Date=model.Date,
             EmployeeId=model.EmployeeId,
             Gift=model.Gift,
             Month=model.Month,
             Price=model.Price
            });
            return RedirectToAction();
        }
        [HttpGet]
        public async Task<ActionResult> EditAward(Guid id)
        {
            var award =await _awardRepository.All()
                                             .AsSplitQuery()
                                             .Include(s=>s.Employee)
                                             .SingleOrDefaultAsync(s=>s.Id==id)
                                             .ConfigureAwait(false);
            if (award != null)
            {
                ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
                return View(new AwardModel
                { 
                 AwardTitle=award.AwardTitle,
                 CreatedBy=award.CreatedBy,
                 CreatedUtc=award.CreatedUtc,
                 Date=award.Date,
                 EmployeeId=award.EmployeeId,
                 EmployeeModel=new EmployeeModel
                 {
                     Name=award.Employee.Name,
                     Id =award.Employee.Id,

                 },
                 Gift=award.Gift,
                 Id=award.Id,
                 IsDelete=award.IsDelete,
                 LastModifiedBy=award.LastModifiedBy,
                 LastModifiedUtc=award.LastModifiedUtc,
                 Month=award.Month,
                 Price=award.Price

                });
            }
            return RedirectToAction(nameof(AwardList));
        }
        public ActionResult EditAward(AwardModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
                return View(model);
            }
            var award = _awardRepository.Find(model.Id);
            award.Gift = model.Gift;
            award.Price = model.Price;
            award.Date = model.Date;
            award.EmployeeId = model.EmployeeId;
            award.Month = model.Date.ToString("MMMM");
            _awardRepository.Update(award, model.Id);
            return RedirectToAction(nameof(AwardList));
        }

        public ActionResult DeleteAward(Guid id)
        {
            var award = _awardRepository.Find(id);
            if (award != null)
            {
                _awardRepository.Delete(award);
                return RedirectToAction(nameof(AwardList));
            }
            return RedirectToAction(nameof(AwardList));
        }
        #endregion

        #region Notice
        [HttpGet]
        public async Task<IActionResult> NoticeList()
        {
            var notices = await _noticeRepository.All().ToListAsync().ConfigureAwait(false);
            return View(notices.Map<List<NoticeModel>>());
        }
        [HttpGet]
        public IActionResult AddNotice()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNotice(NoticeModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _noticeRepository.Insert(model.Map<Notice>());
            return RedirectToAction(nameof(NoticeList));
        }
        [HttpGet]
        public ActionResult EditNotice(Guid id)
        {
            var notice = _noticeRepository.Find(id);
            if (notice != null)
            {
                return View(notice.Map<NoticeModel>());
            }
            return RedirectToAction(nameof(NoticeList));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNotice(NoticeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _noticeRepository.Update(model.Map<Notice>(), model.Id);
            return RedirectToAction(nameof(NoticeList));
        }

        public ActionResult DeleteNotice(Guid id)
        {
            var notice = _noticeRepository.Find(id);
            if (notice != null)
            {
                _noticeRepository.Delete(notice);
                return RedirectToAction(nameof(NoticeList));
            }
            return RedirectToAction(nameof(NoticeList));
        }

        #endregion

        #region Leave Type
        [HttpGet]
        public async Task<IActionResult> LeaveTypeList()
        {
            var leaveTypes = await _leaveTypeRepository.All()
                                                              .ToListAsync()
                                                              .ConfigureAwait(false);
            return View(leaveTypes.Map<List<LeaveTypeModel>>());
        }
        [HttpPost]
        public async Task<IActionResult> LeaveTypeList(LeaveTypeModel model)
        {
            var query = _leaveTypeRepository.All();
            List<LeaveType> leaveTypes;
            if (ModelState.IsValid)
            {
                if (model.Id != Guid.Empty)
                {
                    _leaveTypeRepository.Update(model.Map<LeaveType>(), model.Id);
                    return RedirectToAction(nameof(LeaveTypeList));
                }
                if (_leaveTypeRepository.All().Any(x => x.LeaveTypeName == model.LeaveTypeName))
                {
                    ModelState.AddModelError("LeaveTypeName", "Already Exists!");
                    leaveTypes = await query.ToListAsync().ConfigureAwait(false);
                    return View(leaveTypes.Map<List<LeaveTypeModel>>());
                }
                _leaveTypeRepository.Insert(model.Map<LeaveType>());
                return RedirectToAction(nameof(LeaveTypeList));
            }

            leaveTypes = await query.ToListAsync().ConfigureAwait(false);
            return View(leaveTypes.Map<List<LeaveTypeModel>>());
        }
        [HttpGet]
        public IActionResult DeleteLeaveType(Guid id)
        {
            var leaveType = _leaveTypeRepository.Find(id);
            if (leaveType != null)
            {
                _leaveTypeRepository.Delete(leaveType);
                //success
                return RedirectToAction(nameof(LeaveTypeList));
            }
            //failed
            return RedirectToAction(nameof(LeaveTypeList));
        }


        public async Task<IActionResult> LeaveApplicationList()
        {
            var leaveApplications = await _leaveApplicationRepository.All()
                                                             .AsSplitQuery()
                                                             .Include(x => x.Employee)
                                                             .Include(x => x.LeaveType)
                                                             .ToListAsync()
                                                             .ConfigureAwait(false);

            return View(leaveApplications.Map<List<LeaveApplicationModel>>());
        }

        public IActionResult ChangeLeaveStatus(Guid id, string status)
        {
            var leaveApp = _leaveApplicationRepository.Find(id);
            leaveApp.Status = status;
            _leaveApplicationRepository.Update(leaveApp, leaveApp.Id);
            return RedirectToAction(nameof(LeaveApplicationList));
        }

        #endregion

        #region Leave Group
        [HttpGet]
        public IActionResult AddLeaveGroup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddLeaveGroup(LeaveGroupModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _leaveGroupRepository.Insert(model.Map<LeaveGroup>());
            //success message
            return RedirectToAction(nameof(LeaveGroupList));
        }
        public async Task<IActionResult> LeaveGroupList()
        {
            var leaveGroups = await _leaveGroupRepository.All().ToListAsync().ConfigureAwait(false);
            return View(leaveGroups.Map<List<LeaveGroupModel>>());
        }
        [HttpGet]
        public IActionResult EditGroupLeave(Guid id)
        {
            var leaveGroup = _leaveGroupRepository.Find(id);
            if (leaveGroup != null) return View(leaveGroup.Map<LeaveGroupModel>());
            return RedirectToAction(nameof(LeaveGroupList));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGroupLeave(LeaveGroupModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _leaveGroupRepository.Update(model.Map<LeaveGroup>(), model.Id);
            //success
            return RedirectToAction(nameof(LeaveGroupList));
        }

        public IActionResult DeleteLeaveGroup(Guid id)
        {
            var leaveGroup = _leaveGroupRepository.Find(id);
            if (leaveGroup != null)
            {
                _leaveGroupRepository.Delete(leaveGroup);
                //success
                return RedirectToAction(nameof(LeaveGroupList));
            }
            //fail
            return RedirectToAction(nameof(LeaveGroupList));
        }

        #endregion

        #region LeaveGroupMapping

        public async Task<IActionResult> LeaveGroupMappingList()
        {
            var leaveEmployees = await _leaveEmployeeRepository.All()
                                                              .AsSplitQuery()
                                                              .Include(x => x.Employee)
                                                              .Include(x => x.LeaveGroup)
                                                              .ToListAsync()
                                                              .ConfigureAwait(false);

            return View(leaveEmployees.Map<List<LeaveEmployeeModel>>());
        }
        [HttpGet]
        public IActionResult AddLeaveGroupMapping()
        {
            ViewBag.Employees = _employeeRepository.GetAllEmployeeExceptMappingForDropDown();
            ViewBag.LeaveGroups = _leaveGroupRepository.GetAllLeaveGroupForDropDown();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLeaveGroupMapping(LeaveEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
                ViewBag.LeaveGroups = _leaveGroupRepository.GetAllLeaveGroupForDropDown();
                return View(model);
            }
            _leaveEmployeeRepository.Insert(model.Map<LeaveEmployee>());
            return RedirectToAction(nameof(LeaveGroupMappingList));
        }

        [HttpGet]
        public IActionResult EditLeaveGroupEmployee(Guid id)
        {
            var leaveEmployee = _leaveEmployeeRepository.Find(id);
            if (leaveEmployee != null)
            {
                ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
                ViewBag.LeaveGroups = _leaveGroupRepository.GetAllLeaveGroupForDropDown();
                return View(leaveEmployee.Map<LeaveEmployeeModel>());
            }
            return RedirectToAction(nameof(LeaveGroupMappingList));
        }
        [HttpPost]
        public IActionResult EditLeaveGroupEmployee(LeaveEmployeeModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _employeeRepository.GetAllEmployeeForDropDown();
                ViewBag.LeaveGroups = _leaveGroupRepository.GetAllLeaveGroupForDropDown();
                return View(model);
            }
            _leaveEmployeeRepository.Update(model.Map<LeaveEmployee>(), model.Id);
            return RedirectToAction(nameof(LeaveGroupMappingList));
        }
        #endregion
    }
}
