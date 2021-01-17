using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZimaHrm.Core.ViewModel
{
    public class AttendenceReportViewModel
    {
        public AttendenceReportViewModel()
        {
            StatusViewModel = new List<EmployeeStatustViewModel>();
            AllCurrentMonthDate = new List<string>();
        }
        [DisplayName("Department")]
        [Required]
        public Guid DepartmentId { get; set; }
        [Required]
        public int Month { get; set; }
        public List<string> AllCurrentMonthDate{ get; set; }

        public List<EmployeeStatustViewModel> StatusViewModel { get; set; }
    }
    public class EmployeeStatustViewModel
    {
        public EmployeeStatustViewModel(int days, int month)
        {
            statustViewModel = new List<StatustViewModel>();

            for (int i = 1; i <= days; i++)
            {
                DateTime currentDate = new DateTime(DateTime.Now.Year, month, i);
                this.statustViewModel.Add(new StatustViewModel { Date = currentDate, Status = ""});
            }
        }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public List<StatustViewModel> statustViewModel { get; set; }
    }
    public class StatustViewModel
    {
        public StatustViewModel()
        {

        }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}
