using FinalProjectAmPlansForLoans.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.DataAccess.Repositories
{
    public interface ILoanInputRepository
    {
        Task AddLoanInputIntoDbAsync(LoanInput loanInput);
        Task<IEnumerable<AmPlan>> GetAmortizationPlansByLoanInputIdAsync(int loanInputId);
        Task<LoanInput> GetLoanInputByIdAsync(int loanInputId);
    }
}
