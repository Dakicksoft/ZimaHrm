using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Employee")]
    public class Employee : BaseEntity
    {
        public Employee()
        {
            DateOfBirth = DateTime.Now.AddYears(-30);
            JoiningDate = DateTime.UtcNow;
            ResignDate = DateTime.UtcNow;
            ImagePath = "/images/default_user.png";
        }

        //personal 
        [Required]
        public string Name { get; set; }
        public string Mobile { get; set; }
        [Required]
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }


        //official 
        public double BasicSalary { get; set; }
        public bool Status { get; set; }
        [Required]
        public DateTime JoiningDate { get; set; }
        public DateTime ResignDate { get; set; }

        [ForeignKey("Department")]
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
        
        [ForeignKey("Designation")]
        public Guid DesignationId { get; set; }
        public Designation Designation { get; set; }


        //Bank Account
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
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
