using FinalProjectAmPlansForLoans.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Domain.Models
{
    public class AmPlan : BaseEntity
    {
        [ForeignKey("LoanInputID")]
        public int LoanInputID { get; set; }
        public LoanInput LoanInput { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int NoInstallment { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal Expense { get; set; } 
        public DateTime FirstInstallmentDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }
        public List<decimal> Installments { get; set; } = new List<decimal>();
    }

}
