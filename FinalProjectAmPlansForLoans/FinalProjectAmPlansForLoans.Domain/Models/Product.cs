using FinalProjectAmPlansForLoans.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Domain.Models
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public ProductStatus Status { get; set; }
        public string Description { get; set; }
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
        public double MinInterestRate { get; set; }
        public double MaxInterestRate { get; set; }
        public int MinNumberOfInstallments { get; set; }
        public int MaxNumberOfInstallments { get; set; }
        public double AdminFee { get; set; }
        //[ForeignKey("LoanInputId")]
        //public virtual ICollection<LoanInput> LoanInputs { get; set; }

        //[ForeignKey("AmPlanId")]
        //public virtual ICollection<AmPlan> AmortizationPlans { get; set; }
    }
}
