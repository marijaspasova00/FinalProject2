using FinalProjectAmPlansForLoans.Domain.Models;
using FinalProjectAmPlansForLoans.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectAmPlansForLoans.Services.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<ProductViewModel> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductViewModel productViewModel);
    }
}
