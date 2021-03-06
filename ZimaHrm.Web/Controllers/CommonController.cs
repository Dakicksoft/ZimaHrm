﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZimaHrm.Core.DataModel;
using ZimaHrm.Core.Infrastructure.Mapping;
using ZimaHrm.Data.Entity;
using ZimaHrm.Data.Repository.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZimaHrm.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CommonController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDesignationRepository _designationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<User> _userManager;
        public CommonController(IDepartmentRepository departmentRepository,
                                IDesignationRepository designationRepository,
                                IEmployeeRepository employeeRepository, 
                                UserManager<User> userManager)
        {
            this._departmentRepository = departmentRepository;
            this._designationRepository = designationRepository;
            this._employeeRepository = employeeRepository;
            _userManager = userManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        #region Department

        [HttpGet]
        public async Task<IActionResult> DepartmentList()
        {
            var departments = await _departmentRepository.All()
                                                         .ToListAsync()
                                                         .ConfigureAwait(false);
            return View(departments.Map<List<DepartmentModel>>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DepartmentList(DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != Guid.Empty)
                {
                    _departmentRepository.Update(model.Map<Department>(), model.Id);
                    return RedirectToAction(nameof(DepartmentList));
                }
                _departmentRepository.Insert(model.Map<Department>());
                return RedirectToAction(nameof(DepartmentList));
            }

            var departments = await _departmentRepository.All()
                                                        .ToListAsync()
                                                        .ConfigureAwait(false);
            return View(departments.Map<List<DepartmentModel>>());
        }

        public ActionResult DeleteDeparment(Guid id)
        {
            var dept = _departmentRepository.Find(id);
            if (dept != null)
            {
                _departmentRepository.Delete(dept);
                return RedirectToAction(nameof(DepartmentList));
            }
            return RedirectToAction(nameof(DepartmentList));
        }
        [HttpPost]
        public JsonResult DeptExists(string name, Guid id)
        {
           return Json(!_departmentRepository.AlreadyExist(name, id));
        }
        [HttpPost]
        public JsonResult DesgExists(string name, Guid id)
        {
            return Json(!_designationRepository.AlreadyExist(name, id));
        }
        #endregion

        #region Designation
        [HttpGet]
        public ActionResult AddDesignation()
        {
            ViewBag.Departments = _departmentRepository.GetAllDepartmentForDropDown();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDesignation(DesignationModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _designationRepository.Insert(model.Map<Designation>());
            return RedirectToAction(nameof(DesignationList));
        }
        public async Task<IActionResult> DesignationList()
        {
            var departments = await _departmentRepository.All()
                                                     .AsSplitQuery()
                                                     .Include(x => x.Designations)
                                                     .ToListAsync()
                                                     .ConfigureAwait(false);
            return View(departments.Map<List<DepartmentModel>>());
        }
        [HttpGet]
        public ActionResult EditDesignation(Guid designationId)
        {
            var designation = _designationRepository.Find(designationId);
            if (designation != null)
            {
                ViewBag.Departments = _departmentRepository.GetAllDepartmentForDropDown();
                return View(designation.Map<DesignationModel>());
            }
               
            return RedirectToAction(nameof(DesignationList));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDesignation(DesignationModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _departmentRepository.GetAllDepartmentForDropDown();
                return View(model);
            }
            _designationRepository.Update(model.Map<Designation>(), model.Id);
            return RedirectToAction(nameof(DesignationList));
        }

        public ActionResult DeleteDesignation(Guid id)
        {
            var designation = _designationRepository.Find(id);
            if (designation != null)
            {
                _designationRepository.Delete(designation);
                return RedirectToAction(nameof(DesignationList));
            }
            return RedirectToAction(nameof(DesignationList));
        }

        #endregion

        #region Employee 

        public async Task<ActionResult> EmployeeList()
        {
            var all = await _employeeRepository.All()
                                             .AsNoTracking()
                                             .AsSplitQuery()
                                             .Include(x => x.Designation)
                                             .Include(x => x.Department)
                                             .Select(x=>new EmployeeModel
                                             { 
                                              AccountName=x.AccountName,
                                              AccountNumber=x.AccountNumber,
                                              BasicSalary=x.BasicSalary,
                                              Branch=x.Branch,
                                              CreatedBy=x.CreatedBy,
                                              CreatedUtc=x.CreatedUtc,
                                              CV=x.CV,
                                              DateOfBirth=x.DateOfBirth,
                                              DepartmentId=x.DepartmentId,
                                              DepartmentModel=new DepartmentModel
                                              {
                                                  CreatedBy=x.Department.CreatedBy,
                                                  CreatedUtc=x.Department.CreatedUtc,
                                                  Description=x.Department.Description,
                                                  Id=x.Department.Id,
                                                  IsDelete=x.Department.IsDelete,
                                                  LastModifiedBy=x.Department.LastModifiedBy,
                                                  LastModifiedUtc=x.Department.LastModifiedUtc,
                                                  Name=x.Department.Name
                                              },
                                              DesignationId=x.DesignationId,
                                              DesignationModel=new DesignationModel
                                              { 
                                               CreatedBy=x.Designation.CreatedBy,
                                               CreatedUtc=x.Designation.CreatedUtc,
                                               DepartmentId=x.Designation.DepartmentId,
                                               Id=x.Designation.Id,
                                               IsDelete=x.Designation.IsDelete,
                                               LastModifiedBy=x.Designation.LastModifiedBy,
                                               LastModifiedUtc=x.Designation.LastModifiedUtc,
                                               Name=x.Designation.Name
                                              },
                                              Email=x.Email,
                                              Gender=x.Gender,
                                              Id=x.Id,
                                              ImagePath=x.ImagePath,
                                              IsDelete=x.IsDelete,
                                              JoiningDate=x.JoiningDate,
                                              LastModifiedBy=x.LastModifiedBy,
                                              LastModifiedUtc=x.LastModifiedUtc,
                                              Mobile=x.Mobile,
                                              Name=x.Name,
                                              NationalId=x.NationalId,
                                              Other=x.Other,
                                              PermanentAddress=x.PermanentAddress,
                                              PresentAddress=x.PresentAddress,
                                              ResignDate=x.ResignDate,
                                              Status=x.Status,
                                              SWIFTCode=x.SWIFTCode
                                             })
                                             .ToListAsync()
                                             .ConfigureAwait(false);

            return View(all);
        }
        [HttpGet]
        public async Task<ActionResult> EditEmployee(Guid id)
        {
            var employee =await _employeeRepository.All()
                                                   .AsSplitQuery()
                                                   .AsNoTracking()
                                                   .Include(s=>s.Department)
                                                   .Include(s=>s.Designation)
                                                   .SingleAsync(s=>s.Id==id)
                                                   .ConfigureAwait(false);

            ViewBag.Department = _departmentRepository.GetAllDepartmentForDropDown();
            ViewBag.Designation = _designationRepository.GetAllDesignationForDropDown();


            var model = new EmployeeModel
                {
                    AccountName = employee.AccountName,
                    AccountNumber = employee.AccountNumber,
                    BasicSalary = employee.BasicSalary,
                    Branch = employee.Branch,
                    CreatedBy = employee.CreatedBy,
                    CreatedUtc = employee.CreatedUtc,
                    CV = employee.CV,
                    DateOfBirth = employee.DateOfBirth,
                    DepartmentId = employee.DepartmentId,
                    DepartmentModel = new DepartmentModel
                    {
                        CreatedBy = employee.Department.CreatedBy,
                        CreatedUtc = employee.Department.CreatedUtc,
                        Description = employee.Department.Description,
                        Id = employee.Department.Id,
                        IsDelete = employee.Department.IsDelete,
                        LastModifiedBy = employee.Department.LastModifiedBy,
                        LastModifiedUtc = employee.Department.LastModifiedUtc,
                        Name = employee.Department.Name
                    },
                    DesignationId = employee.DesignationId,
                    DesignationModel = new DesignationModel
                    {
                        CreatedBy = employee.Designation.CreatedBy,
                        CreatedUtc = employee.Designation.CreatedUtc,
                        DepartmentId = employee.Designation.DepartmentId,
                        Id = employee.Designation.Id,
                        IsDelete = employee.Designation.IsDelete,
                        LastModifiedBy = employee.Designation.LastModifiedBy,
                        LastModifiedUtc = employee.Designation.LastModifiedUtc,
                        Name = employee.Designation.Name
                    },
                    Email = employee.Email,
                    Gender = employee.Gender,
                    Id = employee.Id,
                    ImagePath = employee.ImagePath,
                    IsDelete = employee.IsDelete,
                    JoiningDate = employee.JoiningDate,
                    LastModifiedBy = employee.LastModifiedBy,
                    LastModifiedUtc = employee.LastModifiedUtc,
                    Mobile = employee.Mobile,
                    Name = employee.Name,
                    NationalId = employee.NationalId,
                    Other = employee.Other,
                    PermanentAddress = employee.PermanentAddress,
                    PresentAddress = employee.PresentAddress,
                    ResignDate = employee.ResignDate,
                    Status = employee.Status,
                    SWIFTCode = employee.SWIFTCode
                };
            return View(model);
        }

        public ActionResult EditEmployee(EmployeeModel model, IFormFile logoPostedFileBase)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Department = _departmentRepository.GetAllDepartmentForDropDown();
                ViewBag.Designation = _designationRepository.GetAllDesignationForDropDown();
                return View(model);
            }
            var employee = _employeeRepository.Find(model.Id);
            if (logoPostedFileBase != null && logoPostedFileBase.Length > 0)
            {
                var path = Path.Combine(
                 Directory.GetCurrentDirectory(), "wwwroot/Images",
                 logoPostedFileBase.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    logoPostedFileBase.CopyTo(stream);
                }
                model.ImagePath = $"/Images/{logoPostedFileBase.FileName}";
            }else
            {
                model.ImagePath = employee.ImagePath;
            }
            employee.ImagePath = model.ImagePath;
            employee.Name = model.Name;
            employee.Mobile = model.Mobile;
            employee.JoiningDate = model.JoiningDate;
            employee.PermanentAddress = model.PermanentAddress;
            employee.PresentAddress = model.PresentAddress;
            employee.Gender = model.Gender;
            employee.Email = model.Email;
            employee.DesignationId = model.DesignationId;
            employee.DepartmentId = model.DepartmentId;

            employee.BasicSalary = model.BasicSalary;
            employee.ResignDate = model.ResignDate;

            employee.AccountName = model.AccountName;
            employee.AccountNumber = model.AccountNumber;
            employee.Branch = model.Branch; 


            _employeeRepository.Update(employee, employee.Id);
            return RedirectToAction(nameof(EmployeeList));
        }

        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var emp = _employeeRepository.Find(id);
            if (emp == null)
            {
                return RedirectToAction(nameof(EmployeeList));
            }

            _employeeRepository.Delete(emp);
            var user =await _userManager.FindByIdAsync(emp.Id.ToString());

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(EmployeeList));
        }
        [HttpGet]
        public ActionResult AddEmployee()
        {
            ViewBag.Department = _departmentRepository.GetAllDepartmentForDropDown();
            ViewBag.Designation = _designationRepository.GetAllDesignationForDropDown();
            return View(new EmployeeModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddEmployee(EmployeeModel model, IFormFile logoPostedFileBase)
        {

            if (!ModelState.IsValid) return View(model);
            if (logoPostedFileBase != null && logoPostedFileBase.Length > 0)
            {
                var path = Path.Combine(
                 Directory.GetCurrentDirectory(), "wwwroot/Images",
                 logoPostedFileBase.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    logoPostedFileBase.CopyTo(stream);
                }
                model.ImagePath = $"/Images/{logoPostedFileBase.FileName}";
            }

            var domain = new Employee
            {
                AccountName=model.AccountName,
                AccountNumber=model.AccountNumber,
                BasicSalary=model.BasicSalary,
                Branch=model.Branch,
                CV=model.CV,
                DateOfBirth=model.DateOfBirth,
                DepartmentId=model.DepartmentId,
                DesignationId=model.DesignationId,
                Email=model.Email,
                Gender=model.Gender,
                ImagePath=model.ImagePath,
                JoiningDate=model.JoiningDate,
                Mobile=model.Mobile,
                Name=model.Name,
                NationalId=model.NationalId,
                Other=model.Other,
                PermanentAddress=model.PermanentAddress,
                PresentAddress=model.PresentAddress,
                ResignDate=model.ResignDate,
                Status=model.Status,
                SWIFTCode=model.SWIFTCode
            };

            _employeeRepository.Insert(domain);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.Name,
                AvatarURL = "/images/user.png",
                DateRegisteredUTC = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                PhoneNumber = model.Mobile,
                Status = UserStatus.AllGood,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "Pa$$word123!");

            if (!result.Succeeded)
            {
                ViewBag.Msg = "Employee & User not created";

                // If we got this far, something failed, redisplay form
                return View(model);
            }
            //Default role
            await _userManager.AddToRoleAsync(user, Roles.User.ToString());


            ViewBag.Msg = "Employee & User created successfully, Please change the default password (Pa$$word123!) after first login!";
            return RedirectToAction(nameof(EmployeeList));
        }

        public JsonResult DesignationByDepartment(Guid departmentId)
        {
            return Json(_designationRepository.AllDesignationByDepartmentId(departmentId));
        }
        #endregion
    }
}
