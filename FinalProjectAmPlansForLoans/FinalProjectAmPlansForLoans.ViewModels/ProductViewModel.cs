namespace FinalProjectAmPlansForLoans.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; } 
        public string Description { get; set; }
        public double MinAmount { get; set; }
        public double MaxAmount { get; set; }
        public double MinInterestRate { get; set; }
        public double MaxInterestRate { get; set; }
        public int MinNumberOfInstallments { get; set; }
        public int MaxNumberOfInstallments { get; set; }
        public double AdminFee { get; set; }
    }

}
