using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Company")]
    public class Company : BaseEntity
    {
        public string Logo { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Web { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Currency { get; set; }
        [Required]
        public string Address { get; set; }

        public string Tax { get; set; }

        public string Country { get; set; }

        [Required]
        public Subscription Subscription { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SubscriptionExpireDate { get; set; }
    }

    public enum Subscription
    {
        Trial = 0,
        Pro = 1
    }
}
