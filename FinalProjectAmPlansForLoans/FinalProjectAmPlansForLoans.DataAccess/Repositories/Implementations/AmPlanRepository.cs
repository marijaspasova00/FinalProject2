using FinalProjectAmPlansForLoans.DataAccess.DataContext;
using FinalProjectAmPlansForLoans.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.DataAccess.Repositories.Implementations
{
    public class AmPlanRepository : IAmPlanRepository
    {
        private readonly AmPlansDbContext _context;

        public AmPlanRepository(AmPlansDbContext context)
        {
            _context = context;
        }

        public async Task AddAmPlanIntoDbAsync(AmPlan amortizationPlan)
        {
            await _context.AmPlans.AddAsync(amortizationPlan);
            await _context.SaveChangesAsync();
        }

        public async Task AddAmPlanIntoDbListAsync(List<AmPlan> amortizationPlans)
        {
            await _context.AmPlans.AddRangeAsync(amortizationPlans);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<AmPlan> amortizationPlans)
        {
            await _context.AmPlans.AddRangeAsync(amortizationPlans);
            await _context.SaveChangesAsync();
        }

        public async Task<AmPlan> GetAmortizationPlanByLoanInputIdAsync(int loanInputId)
        {
            return await _context.AmPlans
                .FirstOrDefaultAsync(ap => ap.LoanInputID == loanInputId);
        }
    }
}
