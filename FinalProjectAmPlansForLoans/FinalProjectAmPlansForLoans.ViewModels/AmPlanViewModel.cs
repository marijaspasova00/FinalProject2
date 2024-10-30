using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.ViewModels
{
    public class AmPlanViewModel
    {
        public int Id { get; set; }
        public int LoanInputID { get; set; }
        public int ProductID { get; set; }
        public int NoInstallment { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal Expense { get; set; }
        public DateTime ClosingDate { get; set; }
        public string PaymentFrequency { get; set; }
    }

}
