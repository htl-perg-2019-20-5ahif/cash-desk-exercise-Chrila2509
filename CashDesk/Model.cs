using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CashDesk
{
    class Model
    {
        public class Member : IMember
        {
            [Key]
            public int MemberNumber { get; set; }
            [Required]
            [MaxLength(100)]
            public string FirstName { get; set; }
            [Required]
            [MaxLength(100)]
            public string LastName { get; set; }
            [Required]
            public DateTime Birthday { get; set; }

            public Membership Memberships { get; set; }
        }

        public class Membership : IMembership
        {
            [Required]
            public IMember Member { get; set; }
            [Required]
            public DateTime Begin { get; set; }

            public DateTime End { get; set; }

            public List<Deposit> Deposits { get; set; }
        }

        public class Deposit : IDeposit
        {
            [Required]
            public IMembership Membership { get; set; }
            [Required]
            [Range(0, Double.MaxValue)]
            public decimal Amount { get; set; }
        }

        public class DepositStatistics : IDepositStatistics
        {
            public IMember Member { get; set; }

            public int Year { get; set; }

            public decimal TotalAmount { get; set; }
        }
    }
}
