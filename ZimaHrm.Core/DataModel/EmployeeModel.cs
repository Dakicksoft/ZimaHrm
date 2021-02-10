using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    public class EmployeeModel : BaseModel
    {
        public EmployeeModel()
        {
            DateOfBirth = DateTime.Now.AddYears(-30);
            JoiningDate = DateTime.UtcNow;
            ResignDate = DateTime.UtcNow;
            ImagePath = AppData.defaultUser;
        }
        //personal 
        [Required]
        public string Name { get; set; }
        public string Mobile { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        [DisplayName("Present Address")]
        public string PresentAddress { get; set; }
        [DisplayName("Permanent Address")]
        public string PermanentAddress { get; set; }


        //offcial 
        [DisplayName("Basic Salary")]
        public double BasicSalary { get; set; }
        public bool Status { get; set; }
        [DisplayName("Join Date")]
        [Required]
        public DateTime JoiningDate { get; set; }
        [DisplayName("Resign Date")]
        public DateTime ResignDate { get; set; }

        [ForeignKey("DepartmentModel")]
        [DisplayName("Department")]
        public Guid DepartmentId { get; set; }
        public DepartmentModel DepartmentModel { get; set; }
        
        [ForeignKey("DesignationModel")]
        [DisplayName("Designation")]
        public Guid DesignationId { get; set; }
        public DesignationModel DesignationModel { get; set; }


        //Bank Account
        [DisplayName("Account Name")]
        public string AccountName { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        [DisplayName("SWIFT Code")]
        public string SWIFTCode { get; set; }
        public string Branch { get; set; }


        //file or documents 
        public string CV { get; set; }
        public string NationalId{ get; set; }
        public string Other { get; set; }


        //public int? LeaveGroupId { get; set; }
        //public LeaveGroupModel LeaveGroupModel { get; set; }
    }
}
