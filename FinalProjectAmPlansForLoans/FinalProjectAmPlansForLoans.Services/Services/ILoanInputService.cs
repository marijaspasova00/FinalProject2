using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Services.Services
{
    public interface ILoanInputService
    {
        Task AddLoanInputAsync(LoanInputViewModel loanInputViewModel);
        Task AddLoanInputAsync(LoanInput loanInput);
        Task<IEnumerable<AmPlanViewModel>> GetAmortizationPlansByLoanInputAsync(int loanInputId); 

    }
}
