using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectAmPlansForLoans.ViewModels
{
    public class LoanInputViewModel
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime AgreementDate { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestRate { get; set; }
        public string PaymentFrequency { get; set; } 
        public decimal AdminFee { get; set; }
        public DateTime FirstInstallmentDate { get; set; }
        public int NumberOfInstallments { get; set; }
        public DateTime ClosingDate { get; set; }
        public int SelectedProductId { get; set; } 
        public int SelectedPaymentFrequency { get; set; }
        public IEnumerable<SelectListItem> Products { get; set; } 
        public IEnumerable<SelectListItem> PaymentFrequencies { get; set; }
        public IEnumerable<AmPlanViewModel> AmortizationPlans { get; set; } 
    }

}
