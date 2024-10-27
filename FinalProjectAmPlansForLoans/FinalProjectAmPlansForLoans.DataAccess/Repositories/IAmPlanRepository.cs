using FinalProjectAmPlansForLoans.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.DataAccess.Repositories
{
    public interface IAmPlanRepository
    {
        Task AddAmPlanIntoDbAsync(AmPlan amortizationPlan);
        Task AddAmPlanIntoDbListAsync(List<AmPlan> amortizationPlans);
        Task AddRangeAsync(List<AmPlan> amortizationPlans);
        Task<AmPlan> GetAmortizationPlanByLoanInputIdAsync(int loanInputId);
    }
}
