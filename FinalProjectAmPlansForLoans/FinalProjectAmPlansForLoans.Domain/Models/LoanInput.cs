using FinalProjectAmPlansForLoans.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Domain.Models
{
    public class LoanInput : BaseEntity
    {
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public DateTime AgreementDate { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
        public decimal AdminFee { get; set; }
        public DateTime FirstInstallmentDate { get; set; } = DateTime.Today.AddMonths(1);
        public int NumberOfInstallments { get; set; }
        public DateTime ClosingDate { get; set; } 
        public virtual ICollection<AmPlan> AmortizationPlans { get; set; }
    }
}
