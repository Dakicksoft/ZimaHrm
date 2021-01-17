﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ZimaHrm.Core.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ZimaHrm.Data.Repository.Interfaces;
using ZimaHrm.Data.Repository.PaySlip;
using ZimaHrm.Data.Entity;
using ZimaHrm.Core.Infrastructure.Mapping;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZimaHrm.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PayRollController : Controller
    {
        private readonly IAllowanceTypeRepository _allowanceTypeRepository;
        private readonly IAllowanceRepository _allowanceRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAllowanceEmployee _allowanceEmployee;
        private readonly IPaySlipRepository _paySlipRepository;
        private readonly IEmployeePaySlipRepository _employeePaySlipRepository;
        private readonly IPaySlipAllowanceRepository _paySlipAllowanceRepository;

        // GET: /<controller>/
        public PayRollController(IAllowanceTypeRepository allowanceTypeRepository, 
                                 IAllowanceRepository allowanceRepository,
                                 IDepartmentRepository departmentRepository, 
                                 IEmployeeRepository employeeRepository,
                                 IAllowanceEmployee allowanceEmployee, 
                                 IPaySlipRepository paySlipRepository,
                                 IEmployeePaySlipRepository employeePaySlipRepository, 
                                 IPaySlipAllowanceRepository paySlipAllowanceRepository)
        {
            this._allowanceTypeRepository = allowanceTypeRepository;
            this._allowanceRepository = allowanceRepository;
            this._departmentRepository = departmentRepository;
            this._employeeRepository = employeeRepository;
            this._allowanceEmployee = allowanceEmployee;
            this._paySlipRepository = paySlipRepository;
            this._employeePaySlipRepository = employeePaySlipRepository;
            this._paySlipAllowanceRepository = paySlipAllowanceRepository;
        }

        #region Allowance Type List
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllowanceTypeList()
        {
            var allowanceTypes = await _allowanceTypeRepository.All()
                                                               .ToListAsync()
                                                               .ConfigureAwait(false);
            return View(allowanceTypes.Map<List<AllowanceTypeModel>>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllowanceTypeList(AllowanceTypeModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != Guid.Empty)
                {
                    var updateDomain = model.Map<AllowanceType>();
                    _allowanceTypeRepository.Update(updateDomain, model.Id);
                    return RedirectToAction(nameof(AllowanceTypeList));
                }
                if (_allowanceTypeRepository.All().Any(x => x.AllowanceTypeName == model.AllowanceTypeName))
                {
                    ModelState.AddModelError("AllowanceTypeName", "Already Exists!");
                    return View(_allowanceTypeRepository.All());
                }

                var insertDomain = model.Map<AllowanceType>();
                _allowanceTypeRepository.Insert(insertDomain);
                return RedirectToAction(nameof(AllowanceTypeList));
            }
            return View(_allowanceTypeRepository.All());
        }

        public IActionResult DeleteAllowanceType(Guid id)
        {
            var allowanceType = _allowanceTypeRepository.Find(id);
            if (allowanceType != null)
            {
                _allowanceTypeRepository.Delete(allowanceType);
                return RedirectToAction(nameof(AllowanceTypeList));
            }
            return RedirectToAction(nameof(AllowanceTypeList));
        }
        #endregion

        #region Allowance
        [HttpGet]
        public IActionResult AllowanceList(Guid id)

        {
            ViewBag.Depertments = _allowanceTypeRepository.GetAllForDropDown();
            ViewBag.AllowanceList = _allowanceRepository.All();
            if (id != Guid.Empty)
            {
                var allowance = _allowanceRepository.Find(id);
                return View(allowance);
            }
            return View(new AllowanceModel());
        }
        [HttpPost]
        public IActionResult AllowanceList(AllowanceModel model)
        {
            ViewBag.Depertments = _allowanceTypeRepository.GetAllForDropDown();
            ViewBag.AllowanceList = _allowanceRepository.All();
            if (ModelState.IsValid)
            {
                if (model.Id != Guid.Empty)
                {
                    _allowanceRepository.Update(model.Map<Allowance>(), model.Id);
                    //success updated
                    return RedirectToActionPermanent(nameof(AllowanceTypeList), new { id =Guid.Empty });
                }
                if (_allowanceRepository.All().Any(x => x.AllowanceType == model.AllowanceType))
                {
                    ModelState.AddModelError("AllowanceType", "Already Added into allowance List");
                    ViewBag.Depertments = _allowanceTypeRepository.GetAllForDropDown();
                    ViewBag.AllowanceList = _allowanceRepository.All();
                    return View(model);
                }
                _allowanceRepository.Insert(model.Map<Allowance>());
                return RedirectToAction(nameof(AllowanceTypeList));
            }
            return View(model);
        }
        public IActionResult DeleteAllowance(Guid id)
        {
            var allowance = _allowanceRepository.Find(id);
            if (allowance != null)
            {
                _allowanceRepository.Delete(allowance);
                //success
                return RedirectToAction(nameof(AllowanceTypeList));
            }
            //failed not found
            return RedirectToAction(nameof(AllowanceTypeList));
        }
        #endregion

        #region AllowanceEmployee
        [HttpGet]
        public IActionResult EmployeeWithAllowance()
        {
            ViewBag.Depertments = _departmentRepository.GetAllDepertmentForDropDown();
            return View();
        }
        [HttpPost]
        public IActionResult SaveEmployeeAllowance(Guid empId, Guid[] ids)
        {

            if (empId != Guid.Empty && ids != null)
            {
                var allowancesForEmp = _allowanceEmployee.All().Where(x => x.EmployeeId == empId).ToList();
                foreach (var item in allowancesForEmp)
                {
                    _allowanceEmployee.Delete(item);
                }
                foreach (var id in ids)
                {
                    _allowanceEmployee.Insert(new AllowanceEmployeeModel { EmployeeId = empId, AllowanceId = id }.Map<AllowanceEmployee>());
                }
                //success
                return RedirectToAction(nameof(EmployeeWithAllowance));
            }
            return RedirectToAction(nameof(EmployeeWithAllowance));
        }

        [HttpGet]
        public JsonResult EmployeeByDeprt(Guid deptId)
        {
            return Json(_employeeRepository.AllByDepartmentId(deptId));
        }
        [HttpGet]
        public JsonResult AllowanceListByEmployee(Guid deptId, Guid empId)
        {
            var allowances = _allowanceRepository.All();
            var employeeWithAllowances = _allowanceEmployee.All()
                                                           .AsSplitQuery()
                                                           .Include(x => x.Allowance)
                                                           .Where(x => x.EmployeeId == empId);
            foreach (var allowance in allowances)
            {
                foreach (var emp in employeeWithAllowances)
                {
                    if (emp.Allowance == allowance)
                        allowance.isCheck = true;
                }
            }
            return Json(allowances);
        }

        #endregion

        #region PaySlip


        [HttpGet]
        public async Task<IActionResult> PaySlipList()
        {
            var paySlips = await _paySlipRepository.All().ToListAsync().ConfigureAwait(false);
            return View(paySlips.Map<List<PaySlipModel>>());
        }

        [HttpGet]
        public ActionResult CreatePaySlip()
        {
            ViewBag.Departments = _departmentRepository.GetAllDepertmentForDropDown();
            return View();
        }
        [HttpPost]

        public ActionResult CreatePaySlip(PaySlipModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _departmentRepository.GetAllDepertmentForDropDown();
                return View(model);
            }
            model.Month = new DateTime(DateTime.Now.Year, Convert.ToInt16(model.Month), DateTime.Now.Day).ToString("MMMM", CultureInfo.InvariantCulture);
            var employees = new List<EmployeeModel>();
            _paySlipRepository.Insert(model.Map<PaySlip>());
            if (model.DepartmentId != Guid.Empty)
                employees = _employeeRepository.AllByDepartmentId(model.DepartmentId)
                                              .ToList()
                                              .Map<List<EmployeeModel>>();
            else
                employees = _employeeRepository.All()
                                              .ToList()
                                              .Map<List<EmployeeModel>>(); 

            if (employees != null)
            {
                foreach (var item in employees)
                {
                    EmployeePaySlipModel employeePaySlipModel = new EmployeePaySlipModel();
                    List<PaySlipAllowanceModel> paySlipAllowances = new List<PaySlipAllowanceModel>();
                    var allowances = _allowanceEmployee.All()
                                                      .AsSplitQuery()
                                                      .Include(x => x.Employee)
                                                      .Include(x => x.Allowance)
                                                      .Where(x => x.EmployeeId == item.Id).ToList();
                    double totalAllowances = 0;
                    double totalDeduction = 0;
                    double allowance = 0;
                    double deduction = 0;
                    double basicSalary = item.BasicSalary;
                    double amount = 0;
                    foreach (var ad in allowances)
                    {
                        amount = ad.Allowance.IsValue ? ad.Allowance.Value : ((ad.Allowance.Value / 100) * basicSalary);
                        if (ad.Allowance.Type == "Allowance")
                        {
                            allowance = amount;
                            totalAllowances += allowance;
                        }
                        if (ad.Allowance.Type == "Deduction")
                        {
                            deduction = amount;
                            totalDeduction += deduction;
                        }
                        paySlipAllowances.Add(new PaySlipAllowanceModel
                        {
                            AllowanceName = ad.Allowance.AllowanceType,
                            AllowanceType = ad.Allowance.Type,
                            IsValue = ad.Allowance.IsValue,
                            Value = ad.Allowance.Value,
                            Amount = amount
                        });
                    }

                    employeePaySlipModel.AllowanceTotal = totalAllowances;
                    employeePaySlipModel.BasicSalary = item.BasicSalary;
                    employeePaySlipModel.DeductionTotal = totalDeduction;
                    employeePaySlipModel.NetSalary = (item.BasicSalary + totalAllowances) - totalDeduction;
                    employeePaySlipModel.Status = "Paid";
                    employeePaySlipModel.EmployeeId = item.Id;
                    employeePaySlipModel.PaySlipId = model.Id;
                    employeePaySlipModel.PaySlipAllowances = paySlipAllowances;
                    _employeePaySlipRepository.Insert(employeePaySlipModel.Map<EmployeePaySlip>());
                }
            }

            return RedirectToAction(nameof(PaySlipList));
        }

        public ActionResult PaySlipDetails(Guid id)
        {
            var details = _employeePaySlipRepository.All()
                                                   .AsSplitQuery()
                                                   .Include(x => x.Employee)
                                                   .Where(x => x.PaySlipId == id)
                                                   .ToList();
            return View(details);
        }

        public ActionResult DeletePaySlip(Guid id)
        {
            var paySlip = _paySlipRepository.Find(id);
            _paySlipRepository.Delete(paySlip);

            var employeePaySlips = _employeePaySlipRepository.All()
                                                            .Where(x => x.PaySlipId == paySlip.Id);
            foreach (var item in employeePaySlips)
            {
                _employeePaySlipRepository.Delete(item);
            }
            return RedirectToAction(nameof(PaySlipList));
        }

        public ActionResult EmpPaySlipDetails(Guid id)
        {
            var employeePaySlip = _employeePaySlipRepository.All()
                                                           .AsSplitQuery()
                                                           .Include(x => x.Employee)
                                                           .Include(x => x.PaySlip)
                                                           .FirstOrDefault(x => x.Id == id);
            employeePaySlip.PaySlipAllowances = _paySlipAllowanceRepository.All()
                                                                           .Where(x => x.EmployeePaySlipId == employeePaySlip.Id)
                                                                           .ToList();
            return View(employeePaySlip.Map<EmployeePaySlipModel>());
        }

        public ActionResult PrintPaySlip(Guid id)
        {

            var employeePaySlip = _employeePaySlipRepository.All()
                                                           .AsSplitQuery()
                                                           .Include(x => x.Employee)
                                                           .Include(x => x.PaySlip)
                                                           .FirstOrDefault(x => x.Id == id);

            employeePaySlip.PaySlipAllowances = _paySlipAllowanceRepository.All()
                                                                          .Where(x => x.EmployeePaySlipId == employeePaySlip.Id)
                                                                          .ToList();
            //return View(employeePaySlip);
            return new ViewAsPdf("PrintPaySlip", employeePaySlip)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
            //{
            //    PageMargins = { Left = 0, Bottom = 20, Right = 0, Top = 20 },
            //};

        }
        #endregion
    }
}