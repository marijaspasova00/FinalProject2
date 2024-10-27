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
    public class LoanInputRepository : ILoanInputRepository
    {
        private readonly AmPlansDbContext _context;

        public LoanInputRepository(AmPlansDbContext context)
        {
            _context = context;
        }

        public async Task AddLoanInputIntoDbAsync(LoanInput loanInput)
        {
            _context.LoanInputs.AddAsync(loanInput);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AmPlan>> GetAmortizationPlansByLoanInputIdAsync(int loanInputId)
        {
            return await _context.AmPlans
                .Where(amPlan => amPlan.LoanInputID == loanInputId)
                .ToListAsync();
        }

        public async Task<LoanInput> GetLoanInputByIdAsync(int loanInputId)
        {
            return await _context.LoanInputs.FindAsync(loanInputId);
        }
    }
}
